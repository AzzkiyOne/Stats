using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats;

internal sealed class Widget_Table
{
    private readonly List<Column> Columns;
    private readonly List<Widget_TableRow> HeaderRows;
    public List<Widget_TableRow> BodyRows { get; }
    private float TotalHeaderRowsHeight = 0f;
    private float TotalBodyRowsHeight = 0f;
    private Vector2 ScrollPos = new();
    private static Color ColumnSeparatorLineColor = new(1f, 1f, 1f, 0.05f);
    public Widget_Table(
        List<Column> columns,
        List<Widget_TableRow> headerRows,
        List<Widget_TableRow> bodyRows
    )
    {
        Columns = columns;
        HeaderRows = headerRows;
        BodyRows = bodyRows;

        RecalcLayout();
    }
    public void Draw(Rect rect)
    {
        HandleHorScroll(rect);

        // Probably could cache this.
        var leftColumnsMinWidth = 0f;
        var rightColumnsMinWidth = 0f;
        var rightColumnsCount = 0;

        foreach (var column in Columns)
        {
            if (column.IsPinned)
            {
                leftColumnsMinWidth += column.Width;
            }
            else
            {
                rightColumnsMinWidth += column.Width;
                rightColumnsCount++;
            }
        }

        var contentSizeMax = new Vector2(
            // Min. row width
            leftColumnsMinWidth + rightColumnsMinWidth,
            // Total rows height
            TotalBodyRowsHeight + TotalHeaderRowsHeight
        );
        var contentSizeVisible = new Vector2(
            // Will scroll horizontally
            contentSizeMax.y > rect.height
                ? rect.width - GenUI.ScrollBarWidth
                : rect.width,
            // Will scroll vertically
            contentSizeMax.x > rect.width
                ? rect.height - GenUI.ScrollBarWidth
                : rect.height
        );
        var contentRectMax = new Rect(
            Vector2.zero,
            Vector2.Max(contentSizeMax, contentSizeVisible)
        );

        Widgets.BeginScrollView(rect, ref ScrollPos, contentRectMax, true);

        var contentRectVisible = new Rect(ScrollPos, contentSizeVisible);
        // Left part
        var leftPartRect = contentRectVisible.CutByX(leftColumnsMinWidth);

        DrawPart(
            leftPartRect,
            new Vector2(0f, ScrollPos.y),
            0f,
            true
        );

        // Separator line
        Widget_LineVertical.Draw(
            leftPartRect.xMax,
            leftPartRect.y,
            rect.height,
            StatsMainTabWindow.BorderLineColor
        );

        // Right part
        var rightPartFreeSpace = contentRectVisible.width - rightColumnsMinWidth;

        DrawPart(
            contentRectVisible,
            ScrollPos,
            Mathf.Max(rightPartFreeSpace / rightColumnsCount, 0f),
            false
        );

        Widgets.EndScrollView();
    }
    private void DrawPart(
        Rect rect,
        Vector2 scrollPos,
        float cellExtraWidth,
        bool drawPinned
    )
    {
        DrawColumnSeparators(
            rect,
            scrollPos.x,
            cellExtraWidth,
            drawPinned
        );
        DrawHeaders(
            rect.CutByY(TotalHeaderRowsHeight),
            scrollPos.x,
            cellExtraWidth,
            drawPinned
        );
        DrawBody(
            rect,
            scrollPos,
            cellExtraWidth,
            drawPinned
        );
    }
    private void DrawHeaders(
        Rect rect,
        float offsetX,
        float cellExtraWidth,
        bool drawPinned
    )
    {
        GUI.BeginClip(rect);

        var y = 0f;

        for (int i = 0; i < HeaderRows.Count; i++)
        {
            var row = HeaderRows[i];
            row.Draw(
                new Rect(0f, y, rect.width, row.Height),
                offsetX,
                drawPinned,
                cellExtraWidth,
                i,
                this
            );

            y += row.Height;
        }

        GUI.EndClip();
    }
    private void DrawBody(
        Rect rect,
        Vector2 scrollPos,
        float cellExtraWidth,
        bool drawPinned
    )
    {
        GUI.BeginClip(rect);

        var rowRect = new Rect(0f, -scrollPos.y, rect.width, 0f);

        var drawIndex = 0;
        foreach (var row in BodyRows)
        {
            if (rowRect.y >= rect.height) break;
            if (row.IsHidden && row.IsSelected == false) continue;

            rowRect.height = row.Height;

            if (rowRect.yMax > 0f)
            {
                row.Draw(rowRect, scrollPos.x, drawPinned, cellExtraWidth, drawIndex, this);
            }

            rowRect.y = rowRect.yMax;
            drawIndex++;
        }

        GUI.EndClip();
    }
    // The performance impact of instead drawing a vertical border for each
    // individual column is huge. So we have to keep this.
    private void DrawColumnSeparators(
        Rect rect,
        float offsetX,
        float cellExtraWidth,
        bool drawPinned
    )
    {
        if (Event.current.type != EventType.Repaint) return;

        var x = -offsetX;

        foreach (var column in Columns)
        {
            if (column.IsPinned != drawPinned) continue;

            var xMax = x + column.Width + cellExtraWidth;

            if (xMax >= rect.width) break;

            if (xMax > 0f)
            {
                Widget_LineVertical.Draw(
                    xMax + rect.x,
                    rect.y,
                    rect.height,
                    ColumnSeparatorLineColor
                );
            }

            x = xMax;
        }
    }
    private void HandleHorScroll(Rect rect)
    {
        if
        (
            Event.current.isScrollWheel
            &&
            Event.current.control
            &&
            Mouse.IsOver(rect)
        )
        {
            var newScrollX = ScrollPos.x + Event.current.delta.y * 10f;

            ScrollPos.x = newScrollX >= 0f ? newScrollX : 0f;
            Event.current.Use();
        }
    }
    public void RecalcLayout()
    {
        // Reset

        // Columns widths
        foreach (var column in Columns)
        {
            column.Width = 0f;
        }

        // Misc
        TotalHeaderRowsHeight = 0f;
        TotalBodyRowsHeight = 0f;
        ScrollPos.y = 0f;

        // Recalc

        // Header rows
        foreach (var row in HeaderRows)
        {
            TotalHeaderRowsHeight += RecalcRow(row);
        }

        // Body rows
        foreach (var row in BodyRows)
        {
            if (row.IsHidden && row.IsSelected == false) continue;

            TotalBodyRowsHeight += RecalcRow(row);
        }
    }
    private float RecalcRow(Widget_TableRow row)
    {
        foreach (var cell in row.Cells)
        {
            var cellSize = cell.GetSize();
            cell.Column.Width = Mathf.Max(cell.Column.Width, cellSize.x);
            row.Height = Mathf.Max(row.Height, cellSize.y);
        }

        return row.Height;
    }

    public class Column
    {
        public bool IsPinned = false;
        public float Width = 0f;
        public Column(bool isPinned)
        {
            IsPinned = isPinned;
        }
    }
}
