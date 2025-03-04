using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats;

internal class Widget_Table
{
    protected List<Widget_TableRow> HeaderRows { get; } = [];
    protected List<Widget_TableRow> BodyRows { get; } = [];
    private Vector2 ScrollPos = new();
    private static Color ColumnSeparatorLineColor = new(1f, 1f, 1f, 0.05f);
    // ?
    //internal const float FilterWidgetInputInternalPadding = 6f;
    public const float RowHeight = 30f;
    public const float CellPadding = 15f;
    public const float IconGap = 5f;
    public Widget_Table()
    {
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
            if (hCell.IsPinned)
            {
                leftColumnsMinWidth += hCell.Width;
            }
            else
            {
                rightColumnsMinWidth += hCell.Width;
                rightColumnsCount++;
            }
        }

        var minRowWidth = leftColumnsMinWidth + rightColumnsMinWidth;
        var totalRowsHeight = (HeaderRows.Count + BodyRows.Count) * RowHeight;
        var contentSizeMax = new Vector2(minRowWidth, totalRowsHeight);
        var willScrollHor = contentSizeMax.x > targetRect.width;
        var willScrollVer = contentSizeMax.y > targetRect.height;
        var contentSizeVisible = new Vector2(
            willScrollVer
                ? targetRect.width - GenUI.ScrollBarWidth
                : targetRect.width,
            willScrollHor
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
            leftPartRect,
            new Vector2(0f, ScrollPos.y),
            0f,
            static (hCell) => hCell.IsPinned
        );

        // Separator line
        Widget_LineVertical.Draw(
            leftPartRect.xMax,
            leftPartRect.y,
            targetRect.height,
            StatsMainTabWindow.BorderLineColor
        );

        // Right part
        var rightPartFreeSpace = contentRectVisible.width - rightColumnsMinWidth;

        DrawPart(
            contentRectVisible,
            ScrollPos,
            Math.Max(rightPartFreeSpace / rightColumnsCount, 0f),
            static (hCell) => hCell.IsPinned == false
        );

        Widgets.EndScrollView();
    }
    private void DrawPart(
        Rect targetRect,
        Vector2 scrollPos,
        float cellExtraWidth,
        Func<IWidget_TableCell, bool> shouldDrawCell
    )
    {
        DrawColumnSeparators(
            targetRect,
            scrollPos.x,
            cellExtraWidth,
            shouldDrawCell
        );
        DrawHeaders(
            targetRect.CutByY(HeaderRows.Count * RowHeight),
            scrollPos.x,
            cellExtraWidth,
            shouldDrawCell
        );
        DrawBody(
            targetRect,
            scrollPos,
            cellExtraWidth,
            shouldDrawCell
        );
    }
    private void DrawHeaders(
        Rect targetRect,
        float offsetX,
        float cellExtraWidth,
        Func<IWidget_TableCell, bool> shouldDrawCell
    )
    {
        Widgets.BeginGroup(targetRect);

        var y = 0f;

        foreach (var row in HeaderRows)
        {
            var rowRect = new Rect(0f, y, targetRect.width, RowHeight);

            row.Draw(rowRect, offsetX, shouldDrawCell, cellExtraWidth, 0);

            y += rowRect.height;
        }

        Widgets.EndGroup();
    }
    private void DrawBody(
        Rect targetRect,
        Vector2 scrollPos,
        float cellExtraWidth,
        Func<IWidget_TableCell, bool> shouldDrawCell
    )
    {
        Widgets.BeginGroup(targetRect);

        var rowIndexStart = (int)Math.Floor(scrollPos.y / RowHeight);
        var rowIndexEnd = Math.Min(
            (int)Math.Ceiling((scrollPos.y + targetRect.height) / RowHeight),
            BodyRows.Count
        );

        for (int rowIndex = rowIndexStart; rowIndex < rowIndexEnd; rowIndex++)
        {
            var y = rowIndex * RowHeight - scrollPos.y;
            var rowRect = new Rect(0f, y, targetRect.width, RowHeight);
            var row = BodyRows[rowIndex];

            row.Draw(
                rowRect,
                scrollPos.x,
                shouldDrawCell,
                cellExtraWidth,
                rowIndex
            );
        }

        Widgets.EndGroup();
    }
    // The performance impact of instead drawing a vertical border for each
    // individual cell is huge. So we have to keep this.
    private void DrawColumnSeparators(
        Rect targetRect,
        float offsetX,
        float cellExtraWidth,
        Func<IWidget_TableCell, bool> shouldDrawCell
    )
    {
        if (Event.current.type != EventType.Repaint) return;

        var x = -offsetX;

        foreach (var cell in HeaderRows[0].Cells)
        {
            if (shouldDrawCell(cell) == false) continue;

            var cellWidth = cell.Width + cellExtraWidth;
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
