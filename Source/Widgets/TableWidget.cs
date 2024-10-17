using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using TableWidgetRow = (Verse.ThingDef thingDef, Verse.ThingDef? stuffDef);

namespace Stats;

internal sealed class TableWidget
{
    private Vector2 ScrollPosition = new();
    //private readonly List<HeaderCellWidget> Columns;
    private readonly List<ColumnDef> Columns;
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
    private const float HeadersRowHeight = RowHeight;
    public const float CellPadding = 10f;
    public const float CellMinWidth = CellPadding * 2f;
    private static Color ColumnSeparatorLineColor = new(1f, 1f, 1f, 0.05f);
    private readonly List<TableWidgetRow> Rows = [];
    private readonly List<TableWidgetRow> SelectedRows = [];
    private List<TableWidgetRow> CurRows;
    private TableWidgetRow? MouseOverRow = null;
    private readonly TablePart Left;
    private readonly TablePart Right;
    public TableWidget(TableDef tableDef)
    {
        Columns = tableDef.AllColumns;

        var columnsLeft = new List<ColumnDef>();
        var columnsRight = new List<ColumnDef>();

        foreach (var column in tableDef.AllColumns)
        {
            if (column == ColumnDefOf.Id)
            {
                columnsLeft.Add(column);
            }
            else
            {
                columnsRight.Add(column);
            }

            ColumnsWidths[column] =
                CellMinWidth
                + (column.Icon != null ? RowHeight : Text.CalcSize(column.Label).x);
        }

        foreach (var thingDef in DefDatabase<ThingDef>.AllDefs)
        {
            if (
                StatsMod.IsReasonableThingDef(thingDef) == false
                || tableDef.filter(thingDef, null) == false
            )
            {
                continue;
            }

            if (thingDef.MadeFromStuff)
            {
                var allowedStuffs = GenStuff.AllowedStuffsFor(thingDef);

                foreach (var stuffDef in allowedStuffs)
                {
                    AddRow(thingDef, stuffDef);
                }
            }
            else
            {
                AddRow(thingDef);
            }
        }

        // There is an opportunity here to weed out empty columns.
        // If we'll only record widths of columns that returned a cell at least once
        // then after we've processed all of the cells we can look into ColumnsWidths
        // and see what columns are empty.

        SortColumn = columnsLeft.First();
        CurRows = Rows;
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
            CurRows.Count * RowHeight + HeadersRowHeight
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

        Widgets.DrawHighlight(contentRectVisible.TopPartPixels(HeadersRowHeight));

        var curX = contentRectVisible.x;
        var leftPartRect = contentRectVisible.CutFromX(ref curX, Left.MinWidth);
        var signalsLeft = Left.Draw(leftPartRect, new Vector2(0f, ScrollPosition.y));

        LineVerticalWidget.Draw(
            leftPartRect.xMax,
            leftPartRect.y,
            targetRect.height,
            StatsMainTabWindow.BorderLineColor
        );

        var rightPartRect = contentRectVisible.CutFromX(ref curX);
        var signalsRight = Right.Draw(rightPartRect, ScrollPosition);

        Widgets.DrawLineHorizontal(
            contentRectVisible.x,
            contentRectVisible.y + HeadersRowHeight,
            contentRectVisible.width,
            StatsMainTabWindow.BorderLineColor
        );

        Widgets.EndScrollView();

        var clickedRow = signalsLeft.clickedRow ?? signalsRight.clickedRow;
        var clickedColumn = signalsLeft.clickedColumn ?? signalsRight.clickedColumn;

        MouseOverRow = signalsLeft.mouseOverRow ?? signalsRight.mouseOverRow;
        SortColumn = clickedColumn;

        if (clickedRow is TableWidgetRow row)
        {
            if (CurRows == SelectedRows)
            {
                SelectedRows.Remove(row);
            }
            else
            {
                if (SelectedRows.Contains(row))
                {
                    SelectedRows.Remove(row);
                }
                else
                {
                    SelectedRows.Add(row);
                }
            }
        }

        if (Event.current.type == EventType.KeyDown && Event.current.alt)
        {
            if (CurRows == Rows)
            {
                CurRows = SelectedRows;
            }
            else
            {
                CurRows = Rows;
            }
        }
    }
    private void SortRows()
    {
        if (SortColumn != null)
        {
            Rows.Sort((r1, r2) =>
            {
                var r1c = SortColumn.GetCellWidget(r1.thingDef, r1.stuffDef);
                var r2c = SortColumn.GetCellWidget(r2.thingDef, r2.stuffDef);

                if (r1c == null && r2c == null)
                {
                    return 0;
                }

                return (r1c?.CompareTo(r2c) ?? -1) * (int)SortDirection;
            });
        }
    }
    private void AddRow(ThingDef thingDef, ThingDef? stuffDef = null)
    {
        Rows.Add((thingDef, stuffDef));

        foreach (var column in Columns)
        {
            try
            {
                var cell = column.GetCellWidget(thingDef, stuffDef);

                ColumnsWidths[column] = Math.Max(
                    ColumnsWidths[column],
                    cell?.MinWidth ?? 0f
                );
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
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
            Columns = CurFrameColumns = columns;

            foreach (var column in columns)
            {
                MinWidth += parent.ColumnsWidths[column];
            }
        }
        public (
            TableWidgetRow? mouseOverRow,
            TableWidgetRow? clickedRow,
            ColumnDef? clickedColumn
        ) Draw(Rect targetRect, Vector2 scrollPosition)
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
            var clickedColumn = DrawHeaders(
                targetRect.CutFromY(ref curY, HeadersRowHeight),
                cellExtraWidth
            );
            var (mouseOverRow, clickedRow) = DrawBody(
                targetRect.CutFromY(ref curY),
                scrollPosition.y,
                cellExtraWidth
            );

            if (Event.current.type == EventType.Repaint)
            {
                DrawColumnSeparators(targetRect, cellExtraWidth);
            }

            return (mouseOverRow, clickedRow, clickedColumn);
        }
        private ColumnDef? DrawHeaders(Rect targetRect, float cellExtraWidth)
        {
            Widgets.BeginGroup(targetRect);

            var curX = CurFrameCellOffsetX;
            ColumnDef? clickedColumn = null;

            foreach (var column in CurFrameColumns)
            {
                var cellRect = new Rect(
                    curX,
                    0f,
                    Parent.ColumnsWidths[column] + cellExtraWidth,
                    targetRect.height
                );

                curX = cellRect.xMax;

                if (DrawHeaderCell(cellRect, column))
                {
                    clickedColumn = column;
                }
            }

            Widgets.EndGroup();

            return clickedColumn;
        }
        private bool DrawHeaderCell(Rect targetRect, ColumnDef column)
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
                contentRect = column.CellTextAnchor switch
                {
                    TextAnchor.LowerRight => contentRect.RightPartPixels(RowHeight),
                    TextAnchor.LowerCenter => contentRect,
                    _ => contentRect.LeftPartPixels(RowHeight)
                };
                Widgets.DrawTextureFitted(contentRect, column.Icon, 1f);
            }
            else
            {
                Text.Anchor = column.CellTextAnchor;
                Widgets.Label(contentRect, column.Label);
                Text.Anchor = Constants.DefaultTextAnchor;
            }

            TooltipHandler.TipRegion(targetRect, new TipSignal(column.Description));
            Widgets.DrawHighlightIfMouseover(targetRect);

            return Widgets.ButtonInvisible(targetRect);
        }
        private (
            TableWidgetRow? mouseOverRow,
            TableWidgetRow? clickedRow
        ) DrawBody(
            Rect targetRect,
            float scrollPositionY,
            float cellExtraWidth
        )
        {
            Widgets.BeginGroup(targetRect);

            TableWidgetRow? clickedRow = null;
            TableWidgetRow? mouseOverRow = null;
            var rowIndexStart = (int)Math.Floor(scrollPositionY / RowHeight);
            var rowIndexEnd = Math.Min(
                (int)Math.Ceiling((scrollPositionY + targetRect.height) / RowHeight),
                Parent.CurRows.Count
            );

            // Rows
            for (int rowIndex = rowIndexStart; rowIndex < rowIndexEnd; rowIndex++)
            {
                var curY = rowIndex * RowHeight - scrollPositionY;
                var rowRect = new Rect(0f, curY, targetRect.width, RowHeight);
                var row = Parent.CurRows[rowIndex];
                var isMouseOver = Parent.MouseOverRow == row;

                if (Parent.CurRows == Parent.Rows && Parent.SelectedRows.Contains(row))
                {
                    Widgets.DrawHighlightSelected(rowRect);
                }

                if (isMouseOver)
                {
                    Widgets.DrawHighlight(rowRect);
                }
                else if (rowIndex % 2 == 0)
                {
                    Widgets.DrawLightHighlight(rowRect);
                }

                if (Mouse.IsOver(rowRect))
                {
                    mouseOverRow = row;
                }

                var curX = CurFrameCellOffsetX;

                // Cells
                foreach (var column in CurFrameColumns)
                {
                    var cellWidth = Parent.ColumnsWidths[column] + cellExtraWidth;

                    column
                        .GetCellWidget(row.thingDef, row.stuffDef)
                        ?.Draw(new Rect(curX, curY, cellWidth, RowHeight));
                    curX += cellWidth;
                }

                if (Widgets.ButtonInvisible(rowRect))
                {
                    clickedRow = row;
                }
            }

            //Debug.TryDrawUIDebugInfo(targetRect, rowIndexEnd - rowIndexStart + "/" + CurFrameColumns.Count);

            Widgets.EndGroup();

            return (mouseOverRow, clickedRow);
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
