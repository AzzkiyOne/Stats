using System.Collections.Generic;
using UnityEngine;
using Verse;

// Rows/Cells culling can be potentially optimized.
// If our columns witdhs and rows heights are static we can precalculate start/end index from which to start/end draw rows/columns.
// The only issue would be, if the viewport gets resized. But then we can just update cached indices.

namespace Stats;

class Table<RowType>
{
    public Vector2 scrollPosition = new();
    public readonly List<IColumn<RowType>> middleColumns = [];
    public readonly List<IColumn<RowType>> pinnedColumns = [];
    private readonly List<RowType> rows;
    private readonly float middleColumnsWidth = 0f;
    private readonly float pinnedColumnsWidth = 0f;
    private readonly float minRowWidth = 0f;
    private readonly float totalRowsHeight = 0f;
    private int? mouseOverRowIndex = null;
    private IColumn<RowType> sortColumn;
    private SortDirection sortDirection = SortDirection.Ascending;

    public const float rowHeight = 30f;
    public const float headersRowHeight = rowHeight;
    public const float cellPaddingHor = GenUI.Pad;

    public static Color columnSeparatorLineColor = new(1f, 1f, 1f, 0.04f);

    public Table(List<IColumn<RowType>> columns, List<RowType> rows)
    {
        this.rows = rows;

        if (columns[0] != null)
        {
            sortColumn = columns[0];

            SortRows();
        }

        foreach (var column in columns)
        {
            if (column.id == "Label")
            {
                pinnedColumns.Add(column);
                pinnedColumnsWidth += column.minWidth;
            }
            else
            {
                middleColumns.Add(column);
                middleColumnsWidth += column.minWidth;
            }

            minRowWidth += column.minWidth;
        }

        totalRowsHeight = rowHeight * rows.Count;
    }

    // There might be an issue where scroll area is smaller than total columns width.
    // Probably fixed, but should check.
    public void Draw(Rect targetRect)
    {
        using (new GameFontCtx(GameFont.Small))
        using (new TextAnchorCtx(TextAnchor.MiddleLeft))
        {
            var contentRect = new Rect(
                0f,
                0f,
                minRowWidth,
                totalRowsHeight + headersRowHeight
            );

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
                targetRect.width,
                headersRowHeight
            );
            DrawHeaders(headersRect);

            var bodyRect = new Rect(
                scrollPosition.x,
                scrollPosition.y + headersRowHeight,
                targetRect.width,
                targetRect.height - headersRowHeight
            );
            DrawBody(bodyRect);

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
                pinnedColumnsWidth + scrollPosition.x,
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
    private void DrawHeaders(Rect targetRect)
    {
        var currX = targetRect.x;

        // Draw pinned headers
        DrawHeaderColumns(
            targetRect.CutFromX(ref currX, pinnedColumnsWidth),
            pinnedColumns
        );
        // Draw middle headers
        DrawHeaderColumns(
            targetRect.CutFromX(ref currX),
            middleColumns,
            scrollPosition
        );
    }
    private void DrawHeaderColumns(
        Rect targetRect,
        List<IColumn<RowType>> columns,
        Vector2? scrollPosition = null
    )
    {
        Widgets.BeginGroup(targetRect);

        var currX = -scrollPosition?.x ?? 0f;

        foreach (var column in columns)
        {
            var cellRect = new Rect(currX, 0, column.minWidth, targetRect.height);

            AdjustColumnWidthIfLastColumn(targetRect, ref cellRect, columns, column);

            if (column.Draw(cellRect, sortColumn == column ? sortDirection : null))
            {
                HandleHeaderRowCellClick(column);
            }

            currX += column.minWidth;
        }

        Widgets.EndGroup();
    }
    private void DrawBody(Rect targetRect)
    {
        var currX = targetRect.x;

        // Draw pinned rows
        DrawRows(
            targetRect.CutFromX(ref currX, pinnedColumnsWidth),
            pinnedColumns,
            new Vector2(0, scrollPosition.y)
        );
        // Draw middle rows
        DrawRows(
            targetRect.CutFromX(ref currX),
            middleColumns,
            scrollPosition
        );
    }
    private void DrawRows(
        Rect targetRect,
        List<IColumn<RowType>> columns,
        Vector2 scrollPosition
    )
    {
        Widgets.BeginGroup(targetRect);

        float currY = -scrollPosition.y;
        int debug_rowsDrawn = 0;
        int debug_columnsDrawn = 0;

        // Rows
        for (int i = 0; i < rows.Count; i++)
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

            var row = rows[i];
            float currX = -scrollPosition.x;

            // Cells
            foreach (var column in columns)
            {
                // Culling
                if (currX + column.minWidth <= 0)
                {
                    currX += column.minWidth;
                    continue;
                }
                else if (currX > targetRect.width)
                {
                    break;
                }

                var cellRect = new Rect(currX, currY, column.minWidth, rowHeight);

                AdjustColumnWidthIfLastColumn(targetRect, ref cellRect, columns, column);

                column.GetCellFor(row).Draw(cellRect);

                currX += cellRect.width;
                debug_columnsDrawn++;
            }

            var rowRect = new Rect(0, currY, currX, rowHeight);

            if (Mouse.IsOver(rowRect))
            {
                mouseOverRowIndex = i;
            }

            if (mouseOverRowIndex == i)
            {
                Widgets.DrawHighlight(rowRect);
            }
            else if (i % 2 == 0)
            {
                Widgets.DrawLightHighlight(rowRect);
            }

            currY += rowHeight;
            debug_rowsDrawn++;
        }

        Debug.TryDrawUIDebugInfo(targetRect, debug_rowsDrawn + "/" + debug_columnsDrawn);

        Widgets.EndGroup();
    }
    // Maybe it could be done once for a whole column.
    private void AdjustColumnWidthIfLastColumn(
        Rect parentRect,
        ref Rect targetRect,
        List<IColumn<RowType>> columns,
        IColumn<RowType> column
    )
    {
        if (
            column == columns[columns.Count - 1]
            && targetRect.xMax < parentRect.width
        )
        {
            targetRect.xMax = parentRect.width;
        }
    }
    private void HandleHeaderRowCellClick(IColumn<RowType> column)
    {
        if (column == sortColumn)
        {
            sortDirection = (SortDirection)((int)sortDirection * -1);
        }
        else
        {
            sortDirection = SortDirection.Ascending;
            sortColumn = column;
        }

        SortRows();
    }
    private void SortRows()
    {
        rows.Sort((r1, r2) =>
        {
            var r1c = sortColumn.GetCellFor(r1);
            var r2c = sortColumn.GetCellFor(r2);

            return r1c.CompareTo(r2c) * (int)sortDirection;
        });
    }
}

public enum SortDirection
{
    Ascending = 1,
    Descending = -1,
}
