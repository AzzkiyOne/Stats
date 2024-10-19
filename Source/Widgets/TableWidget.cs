using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Stats;

internal sealed class TableWidget
{
    private Vector2 ScrollPosition = new();
    private readonly Dictionary<ColumnDef, float> ColumnsWidths = [];
    private ColumnDef? _sortColumn;
    private ColumnDef? SortColumn
    {
        get => _sortColumn;
        set
        {
            if (value == SortColumn)
            {
                SortDirection = (SortDirection)((int)SortDirection * -1);
            }
            else
            {
                //sortDirection = SortDirection.Ascending;
                _sortColumn = value;
            }

            SortRows();
        }
    }
    private SortDirection SortDirection = SortDirection.Ascending;
    public const float RowHeight = 30f;
    private const float HeadersRowHeight = RowHeight * 2f;
    public const float CellPadding = 15f;
    public const float CellMinWidth = CellPadding * 2f;
    private static Color ColumnSeparatorLineColor = new(1f, 1f, 1f, 0.05f);
    private readonly List<ThingRec> RowsOrig;
    private readonly List<ThingRec> Rows = [];
    private ThingRec? MouseOverRow = null;
    private readonly TablePart Left;
    private readonly TablePart Right;
    private readonly Dictionary<ColumnDef, IFilterWidget> FilterWidgets;
    private readonly List<IFilterWidget> TempFiltersToApply;
    public TableWidget(TableDef tableDef)
    {
        List<ColumnDef> columns = [ColumnDefOf.Name, .. tableDef.columns];
        var columnsLeft = new List<ColumnDef>();
        var columnsRight = new List<ColumnDef>();

        FilterWidgets = new(columns.Count);
        // This of course needs to be a separate instance.
        TempFiltersToApply = new(columns.Count);

        foreach (var column in columns)
        {
            if (column == ColumnDefOf.Name)
            {
                columnsLeft.Add(column);
            }
            else
            {
                columnsRight.Add(column);
            }

            ColumnsWidths[column] =
                CellMinWidth
                + (column.Icon != null ? RowHeight : Text.CalcSize(column.LabelCap).x)
                + 5f;// To accomodate for padding inside filter inputs.
            FilterWidgets[column] = column.Worker.GetFilterWidget();
        }

        if (tableDef.filter != null)
        {
            foreach (var thingDef in DefDatabase<ThingDef>.AllDefs)
            {
                if (tableDef.filter(thingDef) == false)
                {
                    continue;
                }

                if (thingDef.MadeFromStuff)
                {
                    var allowedStuffs = GenStuff.AllowedStuffsFor(thingDef);

                    foreach (var stuffDef in allowedStuffs)
                    {
                        AddRow(new(thingDef, stuffDef), columns);
                    }
                }
                else
                {
                    AddRow(new(thingDef, null), columns);
                }
            }
        }

        // There is an opportunity here to weed out empty columns.
        // If we'll only record widths of columns that returned a cell at least once
        // then after we've processed all of the cells we can look into ColumnsWidths
        // and see what columns are empty.

        RowsOrig = [.. Rows];
        SortColumn = columnsLeft.First();
        Left = new(this, columnsLeft);
        Right = new(this, columnsRight);
    }
    public void Draw(Rect targetRect)
    {
        if (
            Event.current.isScrollWheel
            && Event.current.control
            && Mouse.IsOver(targetRect)
        )
        {
            var scrollAmount = Event.current.delta.y * 10f;
            var newScrollX = ScrollPosition.x + scrollAmount;

            ScrollPosition.x = newScrollX >= 0f ? newScrollX : 0f;
            Event.current.Use();
        }

        var contentSizeMax = new Vector2(
            Left.MinWidth + Right.MinWidth,
            Rows.Count * RowHeight + HeadersRowHeight
        );
        var willScrollHor = contentSizeMax.x > targetRect.width;
        var willScrollVer = contentSizeMax.y > targetRect.height;
        var contentSizeVisible = new Vector2(
            willScrollVer ? targetRect.width - GenUI.ScrollBarWidth : targetRect.width,
            willScrollHor ? targetRect.height - GenUI.ScrollBarWidth : targetRect.height
        );
        var contentRectMax = new Rect(
            Vector2.zero,
            Vector2.Max(contentSizeMax, contentSizeVisible)
        );

        Widgets.BeginScrollView(targetRect, ref ScrollPosition, contentRectMax, true);

        var contentRectVisible = new Rect(ScrollPosition, contentSizeVisible);

        // Headers background stuff.
        Widgets.DrawHighlight(contentRectVisible.TopPartPixels(HeadersRowHeight));
        Widgets.DrawLineHorizontal(
            contentRectVisible.x,
            contentRectVisible.y + HeadersRowHeight / 2f,
            contentRectVisible.width,
            StatsMainTabWindow.BorderLineColor
        );

        var curX = contentRectVisible.x;
        var leftPartRect = contentRectVisible.CutFromX(ref curX, Left.MinWidth);

        Left.Draw(leftPartRect, new Vector2(0f, ScrollPosition.y));

        LineVerticalWidget.Draw(
            leftPartRect.xMax,
            leftPartRect.y,
            targetRect.height,
            StatsMainTabWindow.BorderLineColor
        );

        var rightPartRect = contentRectVisible.CutFromX(curX);

        Right.Draw(rightPartRect, ScrollPosition);

        Widgets.DrawLineHorizontal(
            contentRectVisible.x,
            contentRectVisible.y + HeadersRowHeight,
            contentRectVisible.width,
            StatsMainTabWindow.BorderLineColor
        );

        Widgets.EndScrollView();

        ApplyFiltersIfRequired();

        //if (Event.current.type == EventType.KeyDown && Event.current.alt)
        //{
        //}
    }
    private void SortRows()
    {
        if (SortColumn != null)
        {
            Rows.Sort((r1, r2) =>
            {
                return SortColumn.Worker.Compare(r1, r2) * (int)SortDirection;
            });
            // I think it is a better idea to keep both lists in sync, than to sort
            // current rows every time a user types something in a filter's input
            // field.
            RowsOrig.Sort((r1, r2) =>
            {
                return SortColumn.Worker.Compare(r1, r2) * (int)SortDirection;
            });
        }
    }
    private void AddRow(ThingRec thing, List<ColumnDef> columns)
    {
        Rows.Add(thing);

        foreach (var column in columns)
        {
            try
            {
                var cellMinWidth = column.Worker.GetCellMinWidth(thing);

                ColumnsWidths[column] = Math.Max(
                    ColumnsWidths[column],
                    cellMinWidth ?? 0f
                );
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }
    }
    private void ApplyFiltersIfRequired()
    {
        if (Event.current.type != EventType.Repaint)
        {
            return;
        }

        // Note to myself: please refrain from optimizing this part.
        bool shouldApplyFilters = FilterWidgets
            .Any(widgetEntry => widgetEntry.Value.WasUpdated);

        if (shouldApplyFilters)
        {
            Log.Message("FooBar");
            foreach (var (_, filterWidget) in FilterWidgets)
            {
                if (filterWidget.WasUpdated || filterWidget.HasValue)
                {
                    TempFiltersToApply.Add(filterWidget);
                    filterWidget.WasUpdated = false;
                }
            }

            if (TempFiltersToApply.Count > 0)
            {
                Rows.Clear();

                foreach (var row in RowsOrig)
                {
                    if (TempFiltersToApply.All(filter => filter.Match(row)))
                    {
                        Rows.Add(row);
                    }
                }
            }

            TempFiltersToApply.Clear();
        }
    }
    private sealed class TablePart
    {
        private readonly TableWidget Parent;
        private readonly List<ColumnDef> Columns;
        public float MinWidth { get; } = 0f;
        private float PrevFrameScrollPosX = 0f;
        private float PrevFrameTargetRectWidth = 0f;
        private float CurFrameCellOffsetX = 0f;
        private readonly List<ColumnDef> CurFrameColumns;
        public TablePart(TableWidget parent, List<ColumnDef> columns)
        {
            Parent = parent;
            Columns = columns;
            // We have to make a copy because this list is mutated.
            CurFrameColumns = [.. columns];

            foreach (var column in columns)
            {
                MinWidth += parent.ColumnsWidths[column];
            }
        }
        public void Draw(Rect targetRect, Vector2 scrollPosition)
        {
            var cellExtraWidth = Math.Max(
                (targetRect.width - MinWidth) / Columns.Count,
                0f
            );

            if (
                Event.current.type == EventType.Repaint
                && cellExtraWidth == 0f
                && (
                    PrevFrameScrollPosX != scrollPosition.x
                    || PrevFrameTargetRectWidth != targetRect.width
                )
            )
            {
                UpdateCurFrameState(targetRect, scrollPosition, cellExtraWidth);
            }

            var curY = targetRect.y;

            DrawHeaders(
                targetRect.CutFromY(ref curY, HeadersRowHeight),
                cellExtraWidth
            );
            DrawBody(
                targetRect.CutFromY(curY),
                scrollPosition.y,
                cellExtraWidth
            );

            if (Event.current.type == EventType.Repaint)
            {
                DrawColumnSeparators(targetRect, cellExtraWidth);
            }
        }
        private void DrawHeaders(Rect targetRect, float cellExtraWidth)
        {
            Widgets.BeginGroup(targetRect);

            var curX = CurFrameCellOffsetX;

            foreach (var column in CurFrameColumns)
            {
                var cellRect = new Rect(
                    curX,
                    0f,
                    Parent.ColumnsWidths[column] + cellExtraWidth,
                    targetRect.height
                );

                DrawHeaderCell(cellRect.TopHalf(), column);
                Parent.FilterWidgets[column].Draw(cellRect.BottomHalf());

                curX = cellRect.xMax;
            }

            Widgets.EndGroup();
        }
        private void DrawHeaderCell(Rect targetRect, ColumnDef column)
        {
            if (Parent.SortColumn == column)
            {
                Widgets.DrawBoxSolid(
                    Parent.SortDirection == SortDirection.Ascending
                        ? targetRect.BottomPartPixels(4f)
                        : targetRect.TopPartPixels(4f),
                    Color.yellow
                );
            }

            var contentRect = targetRect.ContractedBy(CellPadding, 0f);

            if (column.Icon != null)
            {
                contentRect = column.style switch
                {
                    ColumnStyle.Number => contentRect.RightPartPixels(RowHeight),
                    ColumnStyle.Boolean => contentRect,
                    _ => contentRect.LeftPartPixels(RowHeight)
                };
                Widgets.DrawTextureFitted(contentRect, column.Icon, 1f);
            }
            else
            {
                Text.Anchor = column.CellTextAnchor;
                Widgets.Label(contentRect, column.LabelCap);
                Text.Anchor = Constants.DefaultTextAnchor;
            }

            TooltipHandler.TipRegion(targetRect, new TipSignal(column.description));
            Widgets.DrawHighlightIfMouseover(targetRect);

            if (Widgets.ButtonInvisible(targetRect))
            {
                Parent.SortColumn = column;
            }
        }
        private void DrawBody(
            Rect targetRect,
            float scrollPositionY,
            float cellExtraWidth
        )
        {
            Widgets.BeginGroup(targetRect);

            var rowIndexStart = (int)Math.Floor(scrollPositionY / RowHeight);
            var rowIndexEnd = Math.Min(
                (int)Math.Ceiling((scrollPositionY + targetRect.height) / RowHeight),
                Parent.Rows.Count
            );

            // Rows
            for (int rowIndex = rowIndexStart; rowIndex < rowIndexEnd; rowIndex++)
            {
                var curY = rowIndex * RowHeight - scrollPositionY;
                var rowRect = new Rect(0f, curY, targetRect.width, RowHeight);
                var row = Parent.Rows[rowIndex];

                //if (Parent.CurRows == Parent.Rows && Parent.SelectedRows.Contains(row))
                //{
                //    Widgets.DrawHighlightSelected(rowRect);
                //}

                if (Mouse.IsOver(rowRect))
                {
                    Parent.MouseOverRow = row;
                }

                var isMouseOver = Parent.MouseOverRow == row;

                if (isMouseOver)
                {
                    Widgets.DrawHighlight(rowRect);
                }
                else if (rowIndex % 2 == 0)
                {
                    Widgets.DrawLightHighlight(rowRect);
                }

                var curX = CurFrameCellOffsetX;

                // Cells
                foreach (var column in CurFrameColumns)
                {
                    var cellWidth = Parent.ColumnsWidths[column] + cellExtraWidth;

                    column
                        .Worker
                        .DrawCell(new Rect(curX, curY, cellWidth, RowHeight), row);
                    curX += cellWidth;
                }

                //if (Widgets.ButtonInvisible(rowRect))
                //{

                //}
            }

            //Debug.TryDrawUIDebugInfo(targetRect, rowIndexEnd - rowIndexStart + "/" + CurFrameColumns.Count);

            Widgets.EndGroup();
        }
        private void DrawColumnSeparators(Rect targetRect, float cellExtraWidth)
        {
            var curSepX = CurFrameCellOffsetX + targetRect.x;
            // Separators
            for (int i = 0; i < CurFrameColumns.Count - 1; i++)
            {
                curSepX += Parent.ColumnsWidths[CurFrameColumns[i]] + cellExtraWidth;

                LineVerticalWidget.Draw(
                    curSepX,
                    targetRect.y,
                    targetRect.height,
                    ColumnSeparatorLineColor
                );
            }
        }
        private void UpdateCurFrameState(
            Rect targetRect,
            Vector2 scrollPosition,
            float cellExtraWidth
        )
        {
            PrevFrameScrollPosX = scrollPosition.x;
            PrevFrameTargetRectWidth = targetRect.width;
            CurFrameCellOffsetX = -scrollPosition.x;
            CurFrameColumns.Clear();

            var curX = CurFrameCellOffsetX;

            for (int i = 0; i < Columns.Count && curX < targetRect.width; i++)
            {
                var column = Columns[i];
                var cellWidth = Parent.ColumnsWidths[column] + cellExtraWidth;

                if (curX + cellWidth <= 0f)
                {
                    curX += cellWidth;
                    CurFrameCellOffsetX = curX;
                    continue;
                }

                curX += cellWidth;
                CurFrameColumns.Add(column);
            }
        }
    }
}

internal enum SortDirection
{
    Ascending = 1,
    Descending = -1,
}
