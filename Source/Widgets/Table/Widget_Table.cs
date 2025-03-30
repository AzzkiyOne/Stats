using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats;

internal class Widget_Table
{
    private readonly List<Widget_TableRow> HeaderRows = [];
    protected List<Widget_TableRow> BodyRows { get; } = [];
    private float TotalHeaderRowsHeight = 0f;
    private float TotalBodyRowsHeight = 0f;
    private Vector2 ScrollPos = new();
    private static Color ColumnSeparatorLineColor = new(1f, 1f, 1f, 0.05f);
    public Widget_Table()
    {
    }
    public void AddHeaderRow(Widget_TableRow row)
    {
        HeaderRows.Add(row);
        TotalHeaderRowsHeight += row.Height;
    }
    public void AddBodyRow(Widget_TableRow row)
    {
        BodyRows.Add(row);
        TotalBodyRowsHeight += row.Height;
    }
    public void Draw(Rect rect)
    {
        HandleHorScroll(ref rect);

        var leftColumnsMinWidth = 0f;
        var rightColumnsMinWidth = 0f;
        var rightColumnsCount = 0;

        foreach (var hCell in HeaderRows[0].Cells)
        {
            if (hCell.Column.IsPinned)
            {
                leftColumnsMinWidth += hCell.Column.Width;
            }
            else
            {
                rightColumnsMinWidth += hCell.Column.Width;
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
            ref leftPartRect,
            new Vector2(0f, ScrollPos.y),
            0f,
            static (cell) => cell.Column.IsPinned
        );

        // Separator line
        Widget_LineVertical.Draw(
            leftPartRect.xMax,
            leftPartRect.y - TotalHeaderRowsHeight,
            rect.height,
            StatsMainTabWindow.BorderLineColor
        );

        // Right part
        var rightPartFreeSpace = contentRectVisible.width - rightColumnsMinWidth;

        DrawPart(
            ref contentRectVisible,
            ScrollPos,
            Math.Max(rightPartFreeSpace / rightColumnsCount, 0f),
            static (cell) => cell.Column.IsPinned == false
        );

        Widgets.EndScrollView();
    }
    private void DrawPart(
        ref Rect rect,
        in Vector2 scrollPos,
        in float cellExtraWidth,
        Func<Widget_TableCell, bool> shouldDrawCell
    )
    {
        DrawColumnSeparators(
            ref rect,
            scrollPos.x,
            cellExtraWidth,
            shouldDrawCell
        );
        var headersRect = rect.CutByY(TotalHeaderRowsHeight);
        DrawHeaders(
            ref headersRect,
            scrollPos.x,
            cellExtraWidth,
            shouldDrawCell
        );
        DrawBody(
            ref rect,
            scrollPos,
            cellExtraWidth,
            shouldDrawCell
        );
    }
    private void DrawHeaders(
        ref Rect rect,
        in float offsetX,
        in float cellExtraWidth,
        Func<Widget_TableCell, bool> shouldDrawCell
    )
    {
        Widgets.BeginGroup(rect);

        var y = 0f;

        foreach (var row in HeaderRows)
        {
            row.Draw(
                new Rect(0f, y, rect.width, row.Height),
                offsetX,
                shouldDrawCell,
                cellExtraWidth,
                0
            );

            y += row.Height;
        }

        Widgets.EndGroup();
    }
    private void DrawBody(
        ref Rect rect,
        in Vector2 scrollPos,
        in float cellExtraWidth,
        Func<Widget_TableCell, bool> shouldDrawCell
    )
    {
        Widgets.BeginGroup(rect);

        var rowRect = new Rect(0f, -scrollPos.y, rect.width, 0f);

        for (int i = 0; i < BodyRows.Count; i++)
        {
            if (rowRect.y >= rect.height) break;

            var row = BodyRows[i];

            rowRect.height = row.Height;

            if (rowRect.yMax > 0f)
            {
                row.Draw(rowRect, scrollPos.x, shouldDrawCell, cellExtraWidth, i);
            }

            rowRect.y = rowRect.yMax;
        }

        Widgets.EndGroup();
    }
    // The performance impact of instead drawing a vertical border for each
    // individual cell is huge. So we have to keep this.
    private void DrawColumnSeparators(
        ref Rect rect,
        in float offsetX,
        in float cellExtraWidth,
        Func<Widget_TableCell, bool> shouldDrawCell
    )
    {
        if (Event.current.type != EventType.Repaint) return;

        var x = -offsetX;

        foreach (var cell in HeaderRows[0].Cells)
        {
            if (shouldDrawCell(cell) == false) continue;

            var cellWidth = cell.Column.Width + cellExtraWidth;
            var xMax = x + cellWidth;

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
    private void HandleHorScroll(ref Rect rect)
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

    public class ColumnProps
    {
        public bool IsPinned { get; set; } = false;
        private float _Width = 0f;
        public float Width
        {
            get => _Width;
            set => _Width = Math.Max(_Width, value);
        }
    }
}
