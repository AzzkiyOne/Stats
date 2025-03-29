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
    public void Draw(Rect targetRect)
    {
        if (
            Event.current.isScrollWheel
            && Event.current.control
            && Mouse.IsOver(targetRect)
        )
        {
            var scrollAmount = Event.current.delta.y * 10f;
            var newScrollX = ScrollPos.x + scrollAmount;

            ScrollPos.x = newScrollX >= 0f ? newScrollX : 0f;
            Event.current.Use();
        }

        var leftColumnsMinWidth = 0f;
        var rightColumnsMinWidth = 0f;
        var rightColumnsCount = 0;

        foreach (var hCell in HeaderRows[0].Cells)
        {
            if (hCell.Props.IsPinned)
            {
                leftColumnsMinWidth += hCell.Props.Width;
            }
            else
            {
                rightColumnsMinWidth += hCell.Props.Width;
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
            contentSizeMax.y > targetRect.height
                ? targetRect.width - GenUI.ScrollBarWidth
                : targetRect.width,
            // Will scroll vertically
            contentSizeMax.x > targetRect.width
                ? targetRect.height - GenUI.ScrollBarWidth
                : targetRect.height
        );
        var contentRectMax = new Rect(
            Vector2.zero,
            Vector2.Max(contentSizeMax, contentSizeVisible)
        );

        Widgets.BeginScrollView(targetRect, ref ScrollPos, contentRectMax, true);

        var contentRectVisible = new Rect(ScrollPos, contentSizeVisible);
        // Left part
        var leftPartRect = contentRectVisible.CutByX(leftColumnsMinWidth);

        DrawPart(
            ref leftPartRect,
            new Vector2(0f, ScrollPos.y),
            0f,
            static (cell) => cell.Props.IsPinned
        );

        // Separator line
        Widget_LineVertical.Draw(
            leftPartRect.xMax,
            leftPartRect.y - TotalHeaderRowsHeight,
            targetRect.height,
            StatsMainTabWindow.BorderLineColor
        );

        // Right part
        var rightPartFreeSpace = contentRectVisible.width - rightColumnsMinWidth;

        DrawPart(
            ref contentRectVisible,
            ScrollPos,
            Math.Max(rightPartFreeSpace / rightColumnsCount, 0f),
            static (cell) => cell.Props.IsPinned == false
        );

        Widgets.EndScrollView();
    }
    private void DrawPart(
        ref Rect targetRect,
        in Vector2 scrollPos,
        in float cellExtraWidth,
        Func<Widget_TableCell, bool> shouldDrawCell
    )
    {
        DrawColumnSeparators(
            ref targetRect,
            scrollPos.x,
            cellExtraWidth,
            shouldDrawCell
        );
        var headersRect = targetRect.CutByY(TotalHeaderRowsHeight);
        DrawHeaders(
            ref headersRect,
            scrollPos.x,
            cellExtraWidth,
            shouldDrawCell
        );
        DrawBody(
            ref targetRect,
            scrollPos,
            cellExtraWidth,
            shouldDrawCell
        );
    }
    private void DrawHeaders(
        ref Rect targetRect,
        in float offsetX,
        in float cellExtraWidth,
        Func<Widget_TableCell, bool> shouldDrawCell
    )
    {
        Widgets.BeginGroup(targetRect);

        var y = 0f;

        foreach (var row in HeaderRows)
        {
            row.Draw(
                new Rect(0f, y, targetRect.width, row.Height),
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
        ref Rect targetRect,
        in Vector2 scrollPos,
        in float cellExtraWidth,
        Func<Widget_TableCell, bool> shouldDrawCell
    )
    {
        Widgets.BeginGroup(targetRect);

        var y = -scrollPos.y;

        for (int i = 0; i < BodyRows.Count; i++)
        {
            if (y >= targetRect.height) break;

            var row = BodyRows[i];
            var yMax = y + row.Height;

            if (yMax > 0f)
            {
                row.Draw(
                    new Rect(0f, y, targetRect.width, row.Height),
                    scrollPos.x,
                    shouldDrawCell,
                    cellExtraWidth,
                    i
                );
            }

            y = yMax;
        }

        Widgets.EndGroup();
    }
    // The performance impact of instead drawing a vertical border for each
    // individual cell is huge. So we have to keep this.
    private void DrawColumnSeparators(
        ref Rect targetRect,
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

            var cellWidth = cell.Props.Width + cellExtraWidth;
            var xMax = x + cellWidth;

            if (xMax >= targetRect.width) break;

            if (xMax > 0f)
            {
                Widget_LineVertical.Draw(
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
