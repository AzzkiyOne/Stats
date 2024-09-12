using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

// Rows/Cells culling can be potentially optimized.
// If our columns witdhs and rows heights are static we can precalculate start/end index from which to start/end draw rows/columns.
// The only issue would be, if the viewport gets resized. But then we can just update cached indices.

namespace Stats.GenTable;

internal class Table<ColumnType, DataType>
    where ColumnType : class, IColumn<DataType>
{
    private Vector2 scrollPosition = new();
    public List<ColumnType> Columns
    {
        set
        {
            pinnedLeftColumns.Clear();
            middleColumns.Clear();
            pinnedLeftColumnsWidth = 0;
            middleColumnsWidth = 0;
            minRowWidth = 0;

            foreach (var column in value)
            {
                if (column.IsPinned)
                {
                    pinnedLeftColumns.Add(column);
                    pinnedLeftColumnsWidth += column.MinWidth;
                }
                else
                {
                    middleColumns.Add(column);
                    middleColumnsWidth += column.MinWidth;
                }

                minRowWidth += column.MinWidth;
            }

            if (SortColumn is null && pinnedLeftColumns.First() != null)
            {
                SortColumn = pinnedLeftColumns.First();
            }
        }
    }
    private readonly List<ColumnType> middleColumns = [];
    private readonly List<ColumnType> pinnedLeftColumns = [];
    private List<Row<ColumnType, DataType>> _rows = [];
    public List<Row<ColumnType, DataType>> Rows
    {
        get
        {
            return _rows;
        }
        set
        {
            _rows = value;
            totalRowsHeight = value.Count * rowHeight;

            SortRows();
        }
    }
    private float middleColumnsWidth = 0f;
    private float pinnedLeftColumnsWidth = 0f;
    private float minRowWidth = 0f;
    private float totalRowsHeight = 0f;
    private int? mouseOverRowIndex = null;
    private ColumnType? _sortColumn;
    private ColumnType? SortColumn
    {
        get
        {
            return _sortColumn;
        }
        set
        {
            if (value == SortColumn)
            {
                sortDirection = (SortDirection)((int)sortDirection * -1);
            }
            else
            {
                //sortDirection = SortDirection.Ascending;
                _sortColumn = value;
            }

            SortRows();
        }
    }
    private SortDirection sortDirection = SortDirection.Ascending;
    private Row<ColumnType, DataType>? selectedRow = null;
    public Row<ColumnType, DataType>? SelectedRow
    {
        get => selectedRow;
        private set
        {
            selectedRow = value;

            foreach (var row in Rows)
            {
                row.Clear();
            }
        }
    }

    private const float rowHeight = 30f;
    private const float headersRowHeight = rowHeight;
    private const float cellPadding = 5f;

    //private static Color columnSeparatorLineColor = new(1f, 1f, 1f, 0.04f);

    public Table(List<ColumnType> columns, List<Row<ColumnType, DataType>> rows)
    {
        Columns = columns;
        Rows = rows;
    }

    public void Draw(Rect targetRect)
    {
        using (new GameFontCtx(GameFont.Small))
        using (new TextAnchorCtx(TextAnchor.MiddleLeft))
        {
            var willHorScroll = totalRowsHeight + headersRowHeight > targetRect.height;
            var adjTargetRectWidth = willHorScroll
                ? targetRect.width - GenUI.ScrollBarWidth
                : targetRect.width;
            var contentRect = new Rect(
                0f,
                0f,
                Math.Max(minRowWidth, adjTargetRectWidth),
                totalRowsHeight + headersRowHeight
            );
            var extraCellWidth = CalcExtraMiddleCellsWidth(adjTargetRectWidth);

            if (
                Event.current.isScrollWheel
                && Event.current.control
                && Mouse.IsOver(targetRect)
            )
            {
                var scrollAmount = Event.current.delta.y * 10;
                var newScrollX = scrollPosition.x + scrollAmount;

                if (newScrollX >= 0)
                {
                    scrollPosition.x = newScrollX;
                }
                else
                {
                    scrollPosition.x = 0;
                }

                Event.current.Use();
            }

            Widgets.BeginScrollView(targetRect, ref scrollPosition, contentRect, true);

            var headersRect = new Rect(
                scrollPosition.x,
                scrollPosition.y,
                adjTargetRectWidth,
                headersRowHeight
            );
            DrawHeaders(headersRect, extraCellWidth);

            var bodyRect = new Rect(
                scrollPosition.x,
                scrollPosition.y + headersRowHeight,
                adjTargetRectWidth,
                targetRect.height - headersRowHeight
            );
            DrawBody(bodyRect, extraCellWidth);

            // Separators
            GUIWidgets.DrawLineVertical(
                scrollPosition.x,
                scrollPosition.y,
                targetRect.height,
                StatsMainTabWindow.borderLineColor
            );
            Widgets.DrawLineHorizontal(
                scrollPosition.x,
                headersRowHeight + scrollPosition.y,
                targetRect.width,
                StatsMainTabWindow.borderLineColor
            );
            GUIWidgets.DrawLineVertical(
                pinnedLeftColumnsWidth + scrollPosition.x,
                scrollPosition.y,
                targetRect.height,
                StatsMainTabWindow.borderLineColor
            );

            if (!Mouse.IsOver(bodyRect))
            {
                mouseOverRowIndex = null;
            }

            Widgets.EndScrollView();
        }
    }
    private void DrawHeaders(Rect targetRect, float extraCellWidth)
    {
        var currX = targetRect.x;

        // Draw pinned headers
        DrawHeaderColumns(
            targetRect.CutFromX(ref currX, pinnedLeftColumnsWidth),
            pinnedLeftColumns,
            Vector2.zero
        );
        // Draw middle headers
        DrawHeaderColumns(
            targetRect.CutFromX(ref currX),
            middleColumns,
            scrollPosition,
            extraCellWidth
        );
    }
    private void DrawHeaderColumns(
        Rect targetRect,
        List<ColumnType> columns,
        Vector2 scrollPosition,
        float extraCellWidth = 0f
    )
    {
        Widgets.BeginGroup(targetRect);

        var currX = -scrollPosition.x;

        foreach (var column in columns)
        {
            var cellRect = new Rect(
                currX,
                0,
                column.MinWidth + extraCellWidth,
                targetRect.height
            );

            if (DrawHeaderCell(cellRect, column))
            {
                SortColumn = column;
            }

            currX += cellRect.width;
        }

        Widgets.EndGroup();
    }
    private bool DrawHeaderCell(Rect targetRect, ColumnType column)
    {
        //Widgets.DrawHighlight(targetRect);
        using (new TextAnchorCtx(column.TextAnchor))
        {
            Widgets.Label(targetRect.ContractedBy(cellPadding, 0), column.Label);
        }

        if (SortColumn == column)
        {
            Widgets.DrawTextureRotated(
                targetRect.RightPartPixels(targetRect.height),
                TexButton.Reveal,
                (int)sortDirection * -90f
            );
        }

        //GUIUtils.DrawLineVertical(
        //    targetRect.xMax,
        //    targetRect.y,
        //    targetRect.height,
        //    Table.columnSeparatorLineColor
        //);

        TooltipHandler.TipRegion(targetRect, new TipSignal(column.Description));

        Widgets.DrawHighlightIfMouseover(targetRect);

        return Widgets.ButtonInvisible(targetRect);
    }
    private void DrawBody(Rect targetRect, float extraCellWidth)
    {
        var currX = targetRect.x;

        // Draw pinned rows
        DrawRows(
            targetRect.CutFromX(ref currX, pinnedLeftColumnsWidth),
            pinnedLeftColumns,
            new Vector2(0, scrollPosition.y)
        );
        // Draw middle rows
        DrawRows(
            targetRect.CutFromX(ref currX),
            middleColumns,
            scrollPosition,
            extraCellWidth
        );
    }
    private void DrawRows(
        Rect targetRect,
        List<ColumnType> columns,
        Vector2 scrollPosition,
        float extraCellWidth = 0f
    )
    {
        Widgets.BeginGroup(targetRect);

        float currY = -scrollPosition.y;
        int debug_rowsDrawn = 0;
        int debug_columnsDrawn = 0;

        // Rows
        for (int i = 0; i < Rows.Count; i++)
        {
            // Culling
            if (currY + rowHeight <= 0)
            {
                currY += rowHeight;
                continue;
            }
            else if (currY >= targetRect.height)
            {
                break;
            }

            debug_columnsDrawn = 0;

            var row = Rows[i];
            var isEven = i % 2 == 0;
            var isMouseOver = mouseOverRowIndex == i;
            var rowRect = new Rect(0, currY, targetRect.width, rowHeight);
            float currX = -scrollPosition.x;

            if (isEven && !isMouseOver)
            {
                Widgets.DrawLightHighlight(rowRect);
            }

            // Cells
            foreach (var column in columns)
            {
                // Culling
                if (currX + column.MinWidth <= 0)
                {
                    currX += column.MinWidth;
                    continue;
                }
                else if (currX > targetRect.width)
                {
                    break;
                }

                var cellRect = new Rect(
                    currX,
                    currY,
                    column.MinWidth + extraCellWidth,
                    rowHeight
                );

                GetRowCell(row, column)?.Draw(cellRect, column.TextAnchor);

                currX += cellRect.width;
                debug_columnsDrawn++;
            }

            if (Mouse.IsOver(rowRect))
            {
                mouseOverRowIndex = i;
            }

            if (Widgets.ButtonInvisible(rowRect))
            {
                if (SelectedRow == row)
                {
                    SelectedRow = null;
                }
                else
                {
                    SelectedRow = row;
                }
            }

            if (SelectedRow == row)
            {
                Widgets.DrawHighlightSelected(rowRect);
            }

            if (isMouseOver)
            {
                Widgets.DrawHighlight(rowRect);
            }

            currY += rowHeight;
            debug_rowsDrawn++;
        }

        Debug.TryDrawUIDebugInfo(targetRect, debug_rowsDrawn + "/" + debug_columnsDrawn);

        Widgets.EndGroup();
    }
    private float CalcExtraMiddleCellsWidth(float parentRectWidth)
    {
        parentRectWidth -= pinnedLeftColumnsWidth;

        if (middleColumnsWidth < parentRectWidth)
        {
            return (parentRectWidth - middleColumnsWidth) / middleColumns.Count;
        }

        return 0f;
    }
    private void SortRows()
    {
        if (SortColumn is null)
        {
            return;
        }

        Rows.Sort((r1, r2) =>
        {
            var r1c = GetRowCell(r1, SortColumn);
            var r2c = GetRowCell(r2, SortColumn);
            int result = 0;

            if (r1c is not null)
            {
                result = r1c.CompareTo(r2c);
            }
            else if (r2c is not null)
            {
                result = -1;
            }

            return result * (int)sortDirection;
        });
    }
    private Cell? GetRowCell(Row<ColumnType, DataType> row, ColumnType column)
    {
        return row.GetCell(column, SelectedRow?.GetCell(column));
    }
}

internal enum SortDirection
{
    Ascending = 1,
    Descending = -1,
}
