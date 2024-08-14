using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

// Rows/Cells culling can be potentially optimized.
// If our columns witdhs and rows heights are static we can precalculate start/end index from which to start/end draw rows/columns.
// The only issue would be, if the viewport gets resized. But then we can just update cached indices.

namespace Stats;

class Table
{
    public const float rowHeight = 30f;
    public const float headersRowHeight = rowHeight;
    public const float cellPaddingHor = 10f;
    public static Color columnSeparatorLineColor = new(1f, 1f, 1f, 0.04f);
    public Vector2 scrollPosition = new();
    public readonly List<Column> middleColumns = [];
    public readonly List<Column> pinnedColumns = [];
    private readonly List<FakeThing> rows;
    private readonly float middleColumnsWidth = 0f;
    private readonly float pinnedColumnsWidth = 0f;
    private readonly float minRowWidth = 0f;
    private readonly float totalRowsHeight = 0f;
    private int? mouseOverRowIndex = null;
    private Column sortColumn;
    private SortDirection sortDirection = SortDirection.Ascending;
    private bool dragInProgress = false;
    public Table(List<Column> columns, List<FakeThing> rows)
    {
        this.rows = rows;

        if (columns[0] != null)
        {
            sortColumn = columns[0];
            sortColumn.SortRows(rows, sortDirection);
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
    // There is an issue where scroll are is smaller than total columns width.
    public void Draw(Rect targetRect)
    {
        using (new GUIUtils.GameFontContext(GameFont.Small))
        using (new GUIUtils.TextAnchorContext(TextAnchor.MiddleLeft))
        {
            var contentRect = new Rect(
                0f,
                0f,
                minRowWidth,
                totalRowsHeight + headersRowHeight
            );
            var controlId = GUIUtility.GetControlID(FocusType.Passive);

            // If the table stops drawing while the user is dragging the mouse on the screen
            // (for example the table is rendered in a window and the user pressed ESC)
            // we end up in a state where dragInProgess == true.
            // I'm not sure if the code below is a good way to fix this, but it looks harmless.
            if (GUIUtility.hotControl == 0)
            {
                dragInProgress = false;
            }

            if (dragInProgress)
            {
                // Taking control over GUI events while dragging is in progress.
                GUIUtility.hotControl = controlId;

                // Catching "mouse up" event to stop drag.
                if (Event.current.GetTypeForControl(controlId) == EventType.MouseUp)
                {
                    GUIUtility.hotControl = 0;
                }
            }

            Widgets.BeginScrollView(targetRect, ref scrollPosition, contentRect, true);

            var pinnedHeadersRowRect = new Rect(
                scrollPosition.x,
                scrollPosition.y,
                pinnedColumnsWidth,
                headersRowHeight
            );
            var headersRowRect = new Rect(
                scrollPosition.x + pinnedColumnsWidth,
                scrollPosition.y,
                targetRect.width - pinnedColumnsWidth,
                rowHeight
            );
            var pinnedTableBodyRect = new Rect(
                scrollPosition.x,
                scrollPosition.y + headersRowHeight,
                pinnedColumnsWidth,
                targetRect.height - headersRowHeight
            );
            var tableBodyRect = new Rect(
                scrollPosition.x + pinnedColumnsWidth,
                scrollPosition.y + headersRowHeight,
                targetRect.width - pinnedColumnsWidth,
                targetRect.height - headersRowHeight
            );

            // Draw pinned headers
            DrawHeaders(pinnedHeadersRowRect, pinnedColumns);
            // Draw headers
            DrawHeaders(headersRowRect, middleColumns, scrollPosition);
            // Draw pinned rows
            DrawRows(pinnedTableBodyRect, pinnedColumns, new Vector2(0, scrollPosition.y));
            // Draw rows
            DrawRows(tableBodyRect, middleColumns, scrollPosition);
            // Separators
            GUIUtils.DrawLineVertical(
                scrollPosition.x,
                scrollPosition.y,
                targetRect.height,
                new(1f, 1f, 1f, 0.4f)
            );
            Widgets.DrawLineHorizontal(
                scrollPosition.x,
                headersRowHeight + scrollPosition.y,
                targetRect.width,
                new(1f, 1f, 1f, 0.4f)
            );
            GUIUtils.DrawLineVertical(
                pinnedColumnsWidth + scrollPosition.x,
                scrollPosition.y,
                targetRect.height,
                new(1f, 1f, 1f, 0.4f)
            );

            // Initiate drag when the user holds left mouse button down in the (not always) scrollable table area.
            if (
                !dragInProgress
                && Event.current.type == EventType.MouseDown
                && Mouse.IsOver(tableBodyRect)
            )
            {
                dragInProgress = true;
            }

            // Adjust horizontal scroll position on drag event.
            if (
                dragInProgress
                && Event.current.GetTypeForControl(controlId) == EventType.MouseDrag
            )
            {
                scrollPosition.x = Mathf.Clamp(
                    scrollPosition.x + Event.current.delta.x,
                    0,
                    contentRect.width - targetRect.width + GUI.skin.verticalScrollbar.fixedWidth
                );
            }

            if (!Mouse.IsOver(pinnedTableBodyRect.Union(tableBodyRect)))
            {
                mouseOverRowIndex = null;
            }

            Widgets.EndScrollView();
        }
    }
    void DrawHeaders(Rect targetRect, List<Column> columns, Vector2? scrollPosition = null)
    {
        Widgets.BeginGroup(targetRect);

        var currX = -scrollPosition?.x ?? 0f;

        foreach (var column in columns)
        {
            var cellRect = new Rect(currX, 0, column.minWidth, targetRect.height);

            cellRect = AdjustColumnWidthIfLastColumn(targetRect, cellRect, columns, column);

            if (column.Draw(cellRect, sortColumn == column ? sortDirection : null))
            {
                HandleHeaderRowCellClick(column);
            }

            currX += column.minWidth;
        }

        Widgets.EndGroup();
    }
    void DrawRows(Rect targetRect, List<Column> columns, Vector2 scrollPosition)
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

                cellRect = AdjustColumnWidthIfLastColumn(targetRect, cellRect, columns, column);


                column.DrawCellFor(cellRect, row);

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
    Rect AdjustColumnWidthIfLastColumn(
        Rect parentRect,
        Rect targetRect,
        List<Column> columns,
        Column column
    )
    {
        if (
            column == columns[columns.Count - 1]
            && targetRect.xMax < parentRect.width
        )
        {
            targetRect.xMax = parentRect.width;
        }

        return targetRect;
    }
    private void HandleHeaderRowCellClick(Column column)
    {
        if (column == null)
        {
            return;
        }

        if (sortColumn == column)
        {
            if (sortDirection == SortDirection.Ascending)
            {
                sortDirection = SortDirection.Descending;
            }
            else
            {
                sortDirection = SortDirection.Ascending;
            }
        }
        else
        {
            sortColumn = column;
            sortDirection = SortDirection.Ascending;
        }

        sortColumn.SortRows(rows, sortDirection);
    }
}

public enum SortDirection
{
    Ascending,
    Descending,
}
