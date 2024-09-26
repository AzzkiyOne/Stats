using System;
using System.Collections.Generic;
using System.Linq;
using Stats.Table.Columns;
using UnityEngine;
using Verse;

// Rows/Cells culling can be potentially optimized.
// If our columns witdhs and rows heights are static we can precalculate start/end index from which to start/end draw rows/columns.
// The only issue would be, if the viewport gets resized. But then we can just update cached indices.

namespace Stats.Table;

internal class Table
{
    private Vector2 ScrollPosition = new();
    private List<Column> _columns;
    public List<Column> Columns
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
                if (column.IsPinned)
                {
                    LeftColumns.Add(column);
                    LeftColumnsWidth += column.MinWidth;
                }
                else
                {
                    MiddleColumns.Add(column);
                    MiddleColumnsWidth += column.MinWidth;
                }

                TotalColumnsMinWidth += column.MinWidth;
            }

            if (SortColumn is null && LeftColumns.First() != null)
            {
                SortColumn = LeftColumns.First();
            }
        }
    }
    private List<IColumn> MiddleColumns { get; } = [];
    private List<IColumn> LeftColumns { get; } = [];
    private float MiddleColumnsWidth { get; set; } = 0f;
    private float LeftColumnsWidth { get; set; } = 0f;
    private float TotalColumnsMinWidth { get; set; } = 0f;
    private float TotalRowsHeight { get; set; } = 0f;
    private int? MouseOverRowIndex { get; set; } = null;
    private IColumn? _sortColumn;
    private IColumn? SortColumn
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
    private Dictionary<IColumn, ICell?>? _selectedRow = null;
    private Dictionary<IColumn, ICell?>? SelectedRow
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
                        var cell = row[column];

                        if (cell is Cells.Cell_Diff diffCell)
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
                        var cell = row[column];
                        var selRowCell = _selectedRow[column];

                        if (cell is Cells.Cell_Diff diffCell)
                        {
                            if (row == _selectedRow || selRowCell == null)
                            {
                                diffCell.Reset();
                            }
                            else if (selRowCell is Cells.Cell_Diff otherCell)
                            {
                                diffCell.Switch(otherCell, column.ReverseDiffModeColors);
                            }
                        }
                    }
                }
            }
        }
    }
    private const float RowHeight = 30f;
    private const float HeadersRowHeight = RowHeight;
    private const float CellPadding = 5f;
    //private static Color ColumnSeparatorLineColor = new(1f, 1f, 1f, 0.1f);
    private List<Dictionary<IColumn, ICell?>> _rows = [];
    public List<Dictionary<IColumn, ICell?>> Rows
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
    public Table(List<ThingAlike> things, List<Column> columns)
    {
        Columns = columns;

        var rows = new List<Dictionary<IColumn, ICell?>>();

        foreach (var thing in things)
        {
            var row = new Dictionary<IColumn, ICell?>(columns.Count);

            foreach (var column in columns)
            {
                try
                {
                    var cell = column.GetCell(thing);

                    if (cell is ICell<float> numCell)
                    {
                        cell = new Cells.Cell_Diff(numCell);
                    }

                    row.Add(column, cell);
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                }
            }

            rows.Add(row);
        }

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
        GUIWidgets.DrawLineVertical(
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
        GUIWidgets.DrawLineVertical(
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
        List<IColumn> columns,
        Vector2 scrollPosition,
        float extraCellWidth = 0f
    )
    {
        Widgets.BeginGroup(targetRect);

        var currX = -scrollPosition.x;

        foreach (var column in columns)
        {
            var columnWidth = column.MinWidth + extraCellWidth;
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
    private bool DrawHeaderCell(Rect targetRect, IColumn column)
    {
        if (SortColumn == column)
        {
            Widgets.DrawBoxSolid(
                SortDirection == SortDirection.Ascending
                    ? targetRect.BottomPartPixels(4f)
                    : targetRect.TopPartPixels(4f),
                Color.yellow
            );
        }

        using (new TextAnchorCtx(column.TextAnchor))
        {
            Widgets.Label(targetRect.ContractedBy(CellPadding, 0), column.Label);
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
        List<IColumn> columns,
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
                var cellWidth = column.MinWidth + extraCellWidth;
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

                row[column]?.Draw(
                    cellRect,
                    cellRect.ContractedBy(CellPadding, 0f),
                    column.TextAnchor
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
            if (r1[SortColumn] == null && r2[SortColumn] == null)
            {
                return 0;
            }

            return (r1[SortColumn]?.CompareTo(r2[SortColumn]) ?? -1) * (int)SortDirection;
        });
    }
}

internal enum SortDirection
{
    Ascending = 1,
    Descending = -1,
}
