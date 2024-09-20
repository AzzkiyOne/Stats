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
    private Vector2 scrollPosition = new();
    public List<Column> Columns
    {
        set
        {
            leftColumns.Clear();
            middleColumns.Clear();
            leftColumnsWidth = 0;
            middleColumnsWidth = 0;
            totalColumnsMinWidth = 0;

            foreach (var column in value)
            {
                if (column.IsPinned)
                {
                    leftColumns.Add(column);
                    leftColumnsWidth += column.MinWidth;
                }
                else
                {
                    middleColumns.Add(column);
                    middleColumnsWidth += column.MinWidth;
                }

                totalColumnsMinWidth += column.MinWidth;
            }

            if (SortColumn is null && leftColumns.First() != null)
            {
                SortColumn = leftColumns.First();
            }
        }
    }
    private readonly List<IColumn> middleColumns = [];
    private readonly List<IColumn> leftColumns = [];
    private float middleColumnsWidth = 0f;
    private float leftColumnsWidth = 0f;
    private float totalColumnsMinWidth = 0f;
    private float totalRowsHeight = 0f;
    private int? mouseOverRowIndex = null;
    private IColumn? sortColumn;
    private IColumn? SortColumn
    {
        get => sortColumn;
        set
        {
            if (value == SortColumn)
            {
                sortDirection = (SortDirection)((int)sortDirection * -1);
            }
            else
            {
                //sortDirection = SortDirection.Ascending;
                sortColumn = value;
            }

            SortThings();
        }
    }
    private SortDirection sortDirection = SortDirection.Ascending;
    private Dictionary<IColumn, ICell?>? selectedRow = null;
    private Dictionary<IColumn, ICell?>? SelectedRow
    {
        get => selectedRow;
        set
        {
            selectedRow = value;
        }
    }
    private const float rowHeight = 30f;
    private const float headersRowHeight = rowHeight;
    private const float cellPadding = 5f;
    //private static Color columnSeparatorLineColor = new(1f, 1f, 1f, 0.1f);
    private List<Dictionary<IColumn, ICell?>> rows = [];
    public List<Dictionary<IColumn, ICell?>> Rows
    {
        get => rows;
        set
        {
            rows = value;
            totalRowsHeight = value.Count * rowHeight;

            // This can cause double sorting.
            SortThings();
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
                    row.Add(column, column.GetCell(thing));
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
        var willHorScroll = totalRowsHeight + headersRowHeight > targetRect.height;
        var adjTargetRectWidth = willHorScroll
            ? targetRect.width - GenUI.ScrollBarWidth
            : targetRect.width;
        var contentRect = new Rect(
            0f,
            0f,
            Math.Max(totalColumnsMinWidth, adjTargetRectWidth),
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
            leftColumnsWidth + scrollPosition.x,
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
    private void DrawHeaders(Rect targetRect, float extraCellWidth)
    {
        var currX = targetRect.x;

        Widgets.DrawHighlight(targetRect);

        // Draw pinned headers
        DrawHeaderColumns(
            targetRect.CutFromX(ref currX, leftColumnsWidth),
            leftColumns,
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
                sortDirection == SortDirection.Ascending
                    ? targetRect.BottomPartPixels(4f)
                    : targetRect.TopPartPixels(4f),
                Color.yellow
            );
        }

        using (new TextAnchorCtx(column.TextAnchor))
        {
            Widgets.Label(targetRect.ContractedBy(cellPadding, 0), column.Label);
        }

        TooltipHandler.TipRegion(targetRect, new TipSignal(column.Description));

        Widgets.DrawHighlightIfMouseover(targetRect);

        //GUIWidgets.DrawLineVertical(
        //    targetRect.xMax,
        //    targetRect.y,
        //    targetRect.height,
        //    columnSeparatorLineColor
        //);

        return Widgets.ButtonInvisible(targetRect);
    }
    private void DrawBody(Rect targetRect, float extraCellWidth)
    {
        var currX = targetRect.x;

        // Draw pinned rows
        DrawRows(
            targetRect.CutFromX(ref currX, leftColumnsWidth),
            leftColumns,
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
        //        columnSeparatorLineColor
        //    );
        //}

        float currY = -scrollPosition.y;
        int debug_rowsDrawn = 0;
        int debug_columnsDrawn = 0;

        // Rows
        for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
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

            var row = rows[rowIndex];
            var isEven = rowIndex % 2 == 0;
            var isMouseOver = mouseOverRowIndex == rowIndex;
            var rowRect = new Rect(0, currY, targetRect.width, rowHeight);
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
                    rowHeight
                );

                row[column]?.Draw(
                    cellRect,
                    cellRect.ContractedBy(cellPadding, 0f),
                    column.TextAnchor
                );

                currX += cellRect.width;
                debug_columnsDrawn++;
            }

            if (Mouse.IsOver(rowRect))
            {
                mouseOverRowIndex = rowIndex;
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

            currY += rowRect.height;
            debug_rowsDrawn++;
        }

        //Debug.TryDrawUIDebugInfo(targetRect, debug_rowsDrawn + "/" + debug_columnsDrawn);

        Widgets.EndGroup();
    }
    private float CalcExtraMiddleCellsWidth(float parentRectWidth)
    {
        parentRectWidth -= leftColumnsWidth;

        if (middleColumnsWidth < parentRectWidth)
        {
            return (parentRectWidth - middleColumnsWidth) / middleColumns.Count;
        }

        return 0f;
    }
    private void SortThings()
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

            return (r1[SortColumn]?.CompareTo(r2[SortColumn]) ?? -1) * (int)sortDirection;
        });
    }
}

internal enum SortDirection
{
    Ascending = 1,
    Descending = -1,
}
