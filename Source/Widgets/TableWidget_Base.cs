using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Stats;

internal abstract class TableWidget_Base
{
    private Vector2 ScrollPosition = new();
    private readonly Dictionary<ColumnDef, float> ColumnsWidths;
    private ColumnDef? _sortColumn;
    protected ColumnDef? SortColumn
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
    public const float IconGap = 5f;
    public const float CellMinWidth = CellPadding * 2f;
    private static Color ColumnSeparatorLineColor = new(1f, 1f, 1f, 0.05f);
    protected List<ThingRec> RowsAll { get; } = [];
    protected List<ThingRec> RowsCur { get; } = [];
    private ThingRec? MouseOverRow;
    protected TablePart Left { get; init; }
    protected TablePart Right { get; init; }
    private readonly Dictionary<ColumnDef, IFilterWidget> FilterWidgets;
    private readonly List<IFilterWidget> TempFiltersToApply;
    private const float FilterWidgetInputInternalPadding = 6f;
    protected List<ColumnDef> Columns { get; }
    public TableWidget_Base(List<ColumnDef> columns)
    {
        Columns = columns;
        SortColumn = columns.First();
        ColumnsWidths = new(Columns.Count);
        FilterWidgets = new(Columns.Count);
        // This of course needs to be a separate instance.
        TempFiltersToApply = new(Columns.Count);

        InitFilterWidgets();

        // There is/was an opportunity somewhere around here to weed out empty columns.
        // If we'll only record widths of columns that returned a cell at least once
        // then after we've processed all of the cells we can look into ColumnsWidths
        // and see what columns are empty.
    }
    protected void InitRows(TableDef tableDef)
    {
        var rows = tableDef.Worker.GetRecords();

        RowsAll.AddRange(rows);
        RowsCur.AddRange(rows);
        SortRows();
    }
    protected void SyncLayout()
    {
        foreach (var column in Columns)
        {
            ColumnsWidths[column] =
                CellMinWidth
                + (column.Icon != null ? RowHeight : Text.CalcSize(column.LabelCap).x)
                + FilterWidgetInputInternalPadding;
        }

        foreach (var row in RowsAll)
        {
            UpdateLayout(row);
        }

        Left.SyncLayout();
        Right.SyncLayout();
    }
    private void InitFilterWidgets()
    {
        foreach (var column in Columns)
        {
            FilterWidgets[column] = column.Worker.GetFilterWidget();
        }
    }
    public virtual void Draw(Rect targetRect)
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
            RowsCur.Count * RowHeight + HeadersRowHeight
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

        if (Event.current.type == EventType.Repaint)
        {
            // Note to myself: please refrain from optimizing this part.
            var wasAnyFilterUpdated = FilterWidgets
                .Any(widgetEntry => widgetEntry.Value.WasUpdated);

            if (wasAnyFilterUpdated)
            {
                ApplyFilters();
            }
        }
    }
    protected void SortRows()
    {
        if (SortColumn != null)
        {
            RowsCur.Sort((r1, r2) =>
            {
                return SortColumn.Worker.Compare(r1, r2) * (int)SortDirection;
            });
            // I think it is a better idea to keep both lists in sync, than to sort
            // current rows every time a user types something in a filter's input
            // field.
            RowsAll.Sort((r1, r2) =>
            {
                return SortColumn.Worker.Compare(r1, r2) * (int)SortDirection;
            });
        }
    }
    protected void UpdateLayout(ThingRec row)
    {
        foreach (var column in Columns)
        {
            try
            {
                var cellMinWidth = (column.Worker.GetCellWidget(row)?.MinWidth ?? 0f) + CellMinWidth;

                ColumnsWidths[column] = Math.Max(
                    ColumnsWidths[column],
                    cellMinWidth
                );
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }
    }
    protected abstract void HandleRowSelect(ThingRec row);
    protected void ApplyFilters()
    {
        foreach (var (_, filterWidget) in FilterWidgets)
        {
            if (filterWidget.WasUpdated || filterWidget.HasValue)
            {
                TempFiltersToApply.Add(filterWidget);
                filterWidget.WasUpdated = false;
            }

            if (TempFiltersToApply.Count > 0)
            {
                RowsCur.Clear();

                foreach (var row in RowsAll)
                {
                    if (TempFiltersToApply.Any(filter => filter.Match(row) == false)) continue;

                    RowsCur.Add(row);
                }
            }

            TempFiltersToApply.Clear();
        }
    }
    protected class TablePart
    {
        protected TableWidget_Base Parent { get; }
        public List<ColumnDef> Columns { get; }
        public float MinWidth { get; private set; } = 0f;
        protected bool ShouldDrawRowAddon { get; init; } = false;
        protected bool ShouldDrawCellAddon { get; init; } = false;
        public TablePart(TableWidget_Base parent, List<ColumnDef> columns)
        {
            Parent = parent;
            Columns = columns;
        }
        public void SyncLayout()
        {
            MinWidth = 0f;

            foreach (var column in Columns)
            {
                MinWidth += Parent.ColumnsWidths[column];
            }
        }
        public void Draw(Rect targetRect, Vector2 scrollPosition)
        {
            var cellExtraWidth = Math.Max(
                (targetRect.width - MinWidth) / Columns.Count,
                0f
            );
            var curY = targetRect.y;

            DrawHeaders(
                targetRect.CutFromY(ref curY, HeadersRowHeight),
                scrollPosition.x,
                cellExtraWidth
            );
            DrawBody(
                targetRect.CutFromY(curY),
                scrollPosition,
                cellExtraWidth
            );
            DrawColumnSeparators(
                targetRect,
                scrollPosition.x,
                cellExtraWidth
            );
        }
        private void DrawHeaders(
            Rect targetRect,
            float scrollPositionX,
            float cellExtraWidth
        )
        {
            Widgets.BeginGroup(targetRect);

            var x = -scrollPositionX;

            foreach (var column in Columns)
            {
                var cellWidth = Parent.ColumnsWidths[column] + cellExtraWidth;

                if (x > targetRect.width) break;

                var xMax = x + cellWidth;

                if (xMax > 0f)
                {
                    var cellRect = new Rect(
                        x,
                        0f,
                        cellWidth,
                        targetRect.height
                    );

                    DrawHeaderCell(cellRect.TopHalf(), column);
                    Parent.FilterWidgets[column].Draw(cellRect.BottomHalf());
                }

                x = xMax;
            }

            Widgets.EndGroup();
        }
        private void DrawHeaderCell(Rect targetRect, ColumnDef column)
        {
            if (Parent.SortColumn == column)
            {
                //Widgets.DrawBoxSolid(
                //    Parent.SortDirection == SortDirection.Ascending
                //        ? targetRect.BottomPartPixels(5f)
                //        : targetRect.TopPartPixels(5f),
                //    Color.yellow
                //);
                Widgets.DrawHighlightSelected(
                    Parent.SortDirection == SortDirection.Ascending
                        ? targetRect.BottomPartPixels(5f)
                        : targetRect.TopPartPixels(5f)
                );
            }

            var contentRect = targetRect.ContractedBy(CellPadding, 0f);

            if (column.Icon != null)
            {
                contentRect = column.Worker.CellStyle switch
                {
                    ColumnCellStyle.Number => contentRect.RightPartPixels(RowHeight),
                    ColumnCellStyle.Boolean => contentRect,
                    _ => contentRect.LeftPartPixels(RowHeight)
                };
                Widgets.DrawTextureFitted(contentRect, column.Icon, 1f);
            }
            else
            {
                Text.Anchor = (TextAnchor)column.Worker.CellStyle;
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
            Vector2 scrollPosition,
            float cellExtraWidth
        )
        {
            Widgets.BeginGroup(targetRect);

            var rowIndexStart = (int)Math.Floor(scrollPosition.y / RowHeight);
            var rowIndexEnd = Math.Min(
                (int)Math.Ceiling((scrollPosition.y + targetRect.height) / RowHeight),
                Parent.RowsCur.Count
            );
            ThingRec? clickedRow = null;

            // RowsCur
            for (int rowIndex = rowIndexStart; rowIndex < rowIndexEnd; rowIndex++)
            {
                var y = rowIndex * RowHeight - scrollPosition.y;
                var rowRect = new Rect(0f, y, targetRect.width, RowHeight);
                var row = Parent.RowsCur[rowIndex];

                if (ShouldDrawRowAddon)
                {
                    DrawRowAddon(rowRect, row);
                }

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

                var x = -scrollPosition.x;

                // Cells
                foreach (var column in Columns)
                {
                    var cellWidth = Parent.ColumnsWidths[column] + cellExtraWidth;

                    if (x > targetRect.width) break;

                    var xMax = x + cellWidth;

                    if (xMax > 0f)
                    {
                        var cellRect = new Rect(x, y, cellWidth, RowHeight);

                        Text.Anchor = (TextAnchor)column.Worker.CellStyle;
                        column.Worker.GetCellWidget(row)?.Draw(cellRect);
                        Text.Anchor = Constants.DefaultTextAnchor;

                        if (ShouldDrawCellAddon)
                        {
                            DrawCellAddon(cellRect, column, row);
                        }
                    }

                    x = xMax;
                }

                if (Widgets.ButtonInvisible(rowRect))
                {
                    clickedRow = row;
                }
            }

            Widgets.EndGroup();

            if (clickedRow != null)
            {
                Parent.HandleRowSelect(clickedRow);
            }
        }
        protected virtual void DrawRowAddon(Rect rowRect, ThingRec row)
        {
        }
        protected virtual void DrawCellAddon(
            Rect cellRect,
            ColumnDef column,
            ThingRec row
        )
        {
        }
        private void DrawColumnSeparators(
            Rect targetRect,
            float scrollPosX,
            float cellExtraWidth
        )
        {
            if (Event.current.type != EventType.Repaint) return;

            var x = -scrollPosX;

            foreach (var column in Columns)
            {
                var cellWidth = Parent.ColumnsWidths[column] + cellExtraWidth;
                var xMax = x + cellWidth;

                if (xMax >= targetRect.width) break;

                if (xMax > 0f)
                {
                    LineVerticalWidget.Draw(
                        xMax + targetRect.x,
                        targetRect.y,
                        targetRect.height,
                        ColumnSeparatorLineColor
                    );
                }

                x = xMax;
            }
        }
    }
}

internal enum SortDirection
{
    Ascending = 1,
    Descending = -1,
}
