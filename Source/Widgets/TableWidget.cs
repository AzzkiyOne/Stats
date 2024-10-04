using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

// Rows/Cells culling can be potentially optimized.
// If our columns witdhs and rows heights are static we can precalculate start/end index from which to start/end draw rows/columns.
// The only issue would be, if the viewport gets resized. But then we can just update cached indices.

namespace Stats;

internal class TableWidget
{
    private Vector2 ScrollPosition = new();
    private List<ColumnDef> _columns;
    public List<ColumnDef> Columns
    {
        private get => _columns;
        set
        {
            _columns = value;

            LeftColumns.Clear();
            MiddleColumns.Clear();
            LeftColumnsWidth = 0;
            MiddleColumnsWidth = 0;
            TotalColumnsMinWidth = 0;

            foreach (var column in value)
            {
                if (column == ColumnDefOf.Id)
                {
                    LeftColumns.Add(column);
                    LeftColumnsWidth += ColumnMinWidths[column];
                }
                else
                {
                    MiddleColumns.Add(column);
                    MiddleColumnsWidth += ColumnMinWidths[column];
                }

                TotalColumnsMinWidth += ColumnMinWidths[column];
            }

            if (SortColumn is null && LeftColumns.First() != null)
            {
                SortColumn = LeftColumns.First();
            }
        }
    }
    private List<ColumnDef> MiddleColumns { get; } = [];
    private List<ColumnDef> LeftColumns { get; } = [];
    private float MiddleColumnsWidth { get; set; } = 0f;
    private float LeftColumnsWidth { get; set; } = 0f;
    private float TotalColumnsMinWidth { get; set; } = 0f;
    private float TotalRowsHeight { get; set; } = 0f;
    private int? MouseOverRowIndex { get; set; } = null;
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
    private SortDirection SortDirection { get; set; } = SortDirection.Ascending;
    private Dictionary<ColumnDef, ICellWidget?>? _selectedRow = null;
    private Dictionary<ColumnDef, ICellWidget?>? SelectedRow
    {
        get => _selectedRow;
        set
        {
            if (value == _selectedRow)
            {
                value = null;
            }

            _selectedRow = value;

            if (_selectedRow == null)
            {
                foreach (var row in Rows)
                {
                    foreach (var column in Columns)
                    {
                        var cell = row.TryGetValue(column);

                        if (cell is CellWidget_Diff diffCell)
                        {
                            diffCell.Reset();
                        }
                    }
                }
            }
            else
            {
                foreach (var row in Rows)
                {
                    foreach (var column in Columns)
                    {
                        var cell = row.TryGetValue(column);
                        var selRowCell = _selectedRow.TryGetValue(column);

                        if (cell is CellWidget_Diff diffCell)
                        {
                            if (row == _selectedRow || selRowCell == null)
                            {
                                diffCell.Reset();
                            }
                            else if (selRowCell is CellWidget_Diff otherCell)
                            {
                                diffCell.Switch(otherCell, !column.BestIsHighest);
                            }
                        }
                    }
                }
            }
        }
    }
    public const float RowHeight = 30f;
    private const float HeadersRowHeight = RowHeight;
    public const float CellPadding = 10f;
    //private static Color ColumnSeparatorLineColor = new(1f, 1f, 1f, 0.1f);
    private List<Dictionary<ColumnDef, ICellWidget?>> _rows = [];
    public List<Dictionary<ColumnDef, ICellWidget?>> Rows
    {
        get => _rows;
        set
        {
            _rows = value;
            TotalRowsHeight = value.Count * RowHeight;

            // This can cause double sorting.
            SortRows();
        }
    }
    private Dictionary<ColumnDef, float> ColumnMinWidths = [];
    public TableWidget(List<ThingAlike> things, List<ColumnDef> columns)
    {
        foreach (var column in columns)
        {
            if (column.Icon != null)
            {
                ColumnMinWidths[column] = RowHeight + CellPadding * 2;
            }
            else
            {
                ColumnMinWidths[column] = Text.CalcSize(column.Label).x + CellPadding * 2;
            }
        }

        var rows = new List<Dictionary<ColumnDef, ICellWidget?>>();

        foreach (var thing in things)
        {
            var row = new Dictionary<ColumnDef, ICellWidget?>(columns.Count);

            foreach (var column in columns)
            {
                try
                {
                    var cell = column.GetCellWidget(thing);

                    if (cell is ICellWidget<float> numCell)
                    {
                        cell = new CellWidget_Diff(numCell);
                    }

                    if (cell != null)
                    {
                        row.Add(column, cell);

                        ColumnMinWidths[column] = Math.Max(ColumnMinWidths[column], cell.MinWidth + CellPadding * 2);
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                }
            }

            rows.Add(row);
        }

        Columns = columns;
        Rows = rows;
    }
    public void Draw(Rect targetRect)
    {
        var totalContentHeight = TotalRowsHeight + HeadersRowHeight;
        var willVerScroll = totalContentHeight > targetRect.height;
        var adjTargetRectWidth = willVerScroll
            ? targetRect.width - GenUI.ScrollBarWidth
            : targetRect.width;
        var contentRect = new Rect(
            0f,
            0f,
            Math.Max(TotalColumnsMinWidth, adjTargetRectWidth),
            totalContentHeight
        );
        var extraCellWidth = CalcExtraMiddleCellsWidth(adjTargetRectWidth);

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

        Widgets.BeginScrollView(targetRect, ref ScrollPosition, contentRect, true);

        var headersRect = new Rect(
            ScrollPosition.x,
            ScrollPosition.y,
            adjTargetRectWidth,
            HeadersRowHeight
        );
        DrawHeaders(headersRect, extraCellWidth);

        var bodyRect = new Rect(
            ScrollPosition.x,
            ScrollPosition.y + HeadersRowHeight,
            adjTargetRectWidth,
            targetRect.height - HeadersRowHeight
        );
        DrawBody(bodyRect, extraCellWidth);

        // Separators
        LineVerticalWidget.Draw(
            ScrollPosition.x,
            ScrollPosition.y,
            targetRect.height,
            StatsMainTabWindow.BorderLineColor
        );
        Widgets.DrawLineHorizontal(
            ScrollPosition.x,
            HeadersRowHeight + ScrollPosition.y,
            targetRect.width,
            StatsMainTabWindow.BorderLineColor
        );
        LineVerticalWidget.Draw(
            LeftColumnsWidth + ScrollPosition.x,
            ScrollPosition.y,
            targetRect.height,
            StatsMainTabWindow.BorderLineColor
        );

        if (!Mouse.IsOver(bodyRect))
        {
            MouseOverRowIndex = null;
        }

        Widgets.EndScrollView();
    }
    private void DrawHeaders(Rect targetRect, float extraCellWidth)
    {
        var currX = targetRect.x;

        Widgets.DrawHighlight(targetRect);

        // Draw pinned headers
        DrawHeaderColumns(
            targetRect.CutFromX(ref currX, LeftColumnsWidth),
            LeftColumns,
            Vector2.zero
        );
        // Draw middle headers
        DrawHeaderColumns(
            targetRect.CutFromX(ref currX),
            MiddleColumns,
            ScrollPosition,
            extraCellWidth
        );
    }
    private void DrawHeaderColumns(
        Rect targetRect,
        List<ColumnDef> columns,
        Vector2 scrollPosition,
        float extraCellWidth = 0f
    )
    {
        Widgets.BeginGroup(targetRect);

        var currX = -scrollPosition.x;

        foreach (var column in columns)
        {
            var columnWidth = ColumnMinWidths[column] + extraCellWidth;
            // Culling
            if (currX + columnWidth <= 0)
            {
                currX += columnWidth;
                continue;
            }
            else if (currX > targetRect.width)
            {
                break;
            }

            var cellRect = new Rect(
                currX,
                0,
                columnWidth,
                targetRect.height
            );

            if (DrawHeaderCell(cellRect, column))
            {
                SortColumn = column;
            }

            currX += columnWidth;
        }

        Widgets.EndGroup();
    }
    private bool DrawHeaderCell(Rect targetRect, ColumnDef column)
    {
        var contentRect = targetRect.ContractedBy(CellPadding, 0);

        if (SortColumn == column)
        {
            Widgets.DrawBoxSolid(
                SortDirection == SortDirection.Ascending
                    ? targetRect.BottomPartPixels(4f)
                    : targetRect.TopPartPixels(4f),
                Color.yellow
            );
        }

        if (column.Icon != null)
        {
            var iconRect = column.CellTextAnchor switch
            {
                TextAnchor.LowerRight => contentRect.RightPartPixels(RowHeight),
                TextAnchor.LowerCenter => contentRect,
                _ => contentRect.LeftPartPixels(RowHeight)
            };

            Widgets.DrawTextureFitted(iconRect, column.Icon, 1f);
        }
        else
        {
            using (new TextAnchorCtx(column.CellTextAnchor))
            {
                Widgets.Label(contentRect, column.Label);
            }
        }

        TooltipHandler.TipRegion(targetRect, new TipSignal(column.Description));

        Widgets.DrawHighlightIfMouseover(targetRect);

        //GUIWidgets.DrawLineVertical(
        //    targetRect.xMax,
        //    targetRect.y,
        //    targetRect.height,
        //    ColumnSeparatorLineColor
        //);

        return Widgets.ButtonInvisible(targetRect);
    }
    private void DrawBody(Rect targetRect, float extraCellWidth)
    {
        var currX = targetRect.x;

        // Draw pinned rows
        DrawRows(
            targetRect.CutFromX(ref currX, LeftColumnsWidth),
            LeftColumns,
            new Vector2(0, ScrollPosition.y)
        );
        // Draw middle rows
        DrawRows(
            targetRect.CutFromX(ref currX),
            MiddleColumns,
            ScrollPosition,
            extraCellWidth
        );
    }
    private void DrawRows(
        Rect targetRect,
        List<ColumnDef> columns,
        Vector2 scrollPosition,
        float extraCellWidth = 0f
    )
    {
        Widgets.BeginGroup(targetRect);

        //float currSepX = -scrollPosition.x;
        //// Separators
        //for (int i = 0; i < columns.Count - 1; i++)
        //{
        //    var columnWidth = columns[i].MinWidth + extraCellWidth;
        //    // Culling
        //    if (currSepX + columnWidth <= 0)
        //    {
        //        currSepX += columnWidth;
        //        continue;
        //    }
        //    else if (currSepX > targetRect.width)
        //    {
        //        break;
        //    }

        //    currSepX += columnWidth;

        //    GUIWidgets.DrawLineVertical(
        //        currSepX,
        //        0f,
        //        targetRect.height,
        //        ColumnSeparatorLineColor
        //    );
        //}

        float currY = -scrollPosition.y;
        int debug_rowsDrawn = 0;

        // Rows
        for (int rowIndex = 0; rowIndex < _rows.Count; rowIndex++)
        {
            // Culling
            if (currY + RowHeight <= 0)
            {
                currY += RowHeight;
                continue;
            }
            else if (currY >= targetRect.height)
            {
                break;
            }

            int debug_columnsDrawn = 0;

            var row = _rows[rowIndex];
            var isEven = rowIndex % 2 == 0;
            var isMouseOver = MouseOverRowIndex == rowIndex;
            var rowRect = new Rect(0, currY, targetRect.width, RowHeight);
            float currX = -scrollPosition.x;

            if (isEven && !isMouseOver)
            {
                Widgets.DrawLightHighlight(rowRect);
            }

            // Cells
            foreach (var column in columns)
            {
                var cellWidth = ColumnMinWidths[column] + extraCellWidth;
                // Culling
                if (currX + cellWidth <= 0)
                {
                    currX += cellWidth;
                    continue;
                }
                else if (currX > targetRect.width)
                {
                    break;
                }

                var cellRect = new Rect(
                    currX,
                    currY,
                    cellWidth,
                    RowHeight
                );

                row.TryGetValue(column)?.Draw(
                    cellRect,
                    cellRect.ContractedBy(CellPadding, 0f),
                    column.CellTextAnchor
                );

                currX += cellRect.width;
                debug_columnsDrawn++;
            }

            if (Mouse.IsOver(rowRect))
            {
                MouseOverRowIndex = rowIndex;
            }

            if (Widgets.ButtonInvisible(rowRect))
            {
                SelectedRow = row;
            }

            if (SelectedRow == row)
            {
                Widgets.DrawHighlightSelected(rowRect);
            }

            if (isMouseOver)
            {
                Widgets.DrawHighlight(rowRect);
            }

            currY += rowRect.height;
            debug_rowsDrawn++;
        }

        //Debug.TryDrawUIDebugInfo(targetRect, debug_rowsDrawn + "/" + debug_columnsDrawn);

        Widgets.EndGroup();
    }
    private float CalcExtraMiddleCellsWidth(float parentRectWidth)
    {
        parentRectWidth -= LeftColumnsWidth;

        if (MiddleColumnsWidth < parentRectWidth)
        {
            return (parentRectWidth - MiddleColumnsWidth) / MiddleColumns.Count;
        }

        return 0f;
    }
    private void SortRows()
    {
        if (SortColumn == null)
        {
            return;
        }

        Rows.Sort((r1, r2) =>
        {
            var r1c = r1.TryGetValue(SortColumn);
            var r2c = r2.TryGetValue(SortColumn);

            if (r1c == null && r2c == null)
            {
                return 0;
            }

            return (r1c?.CompareTo(r2c) ?? -1) * (int)SortDirection;
        });
    }
}

internal enum SortDirection
{
    Ascending = 1,
    Descending = -1,
}
