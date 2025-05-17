using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

internal sealed partial class ObjectTable<TObject>
{
    private readonly float TotalHeaderRowsHeight;
    private float TotalBodyRowsHeight;
    private Vector2 ScrollPosition;
    private static Color ColumnSeparatorLineColor = new(1f, 1f, 1f, 0.05f);
    private const float cellPadHor = 15f;
    private const float cellPadVer = 5f;
    public override void Draw(Rect rect)
    {
        // Why to do it like this?
        //
        // Filters can spam events (though not very often).
        // On each filter's OnChange event we set ShouldApplyFilters flag
        // to then apply filters only once.
        if (ShouldApplyFilters && Event.current.type == EventType.Layout)
        {
            ApplyFilters();
        }

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
            // Total rows totalHeight
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

        Verse.Widgets.BeginScrollView(rect, ref ScrollPosition, contentRectMax, true);

        var contentRectVisible = new Rect(ScrollPosition, contentSizeVisible);

        // Left part
        if (leftColumnsMinWidth > 0f)
        {
            var leftPartRect = contentRectVisible.CutByX(leftColumnsMinWidth);

            DrawPart(leftPartRect, ScrollPosition with { x = 0f }, 0f, true);

            // Separator line
            Widgets.Draw.VerticalLine(
                leftPartRect.xMax - 1f,
                leftPartRect.y,
                rect.height,
                MainTabWindow.BorderLineColor
            );
        }

        // Right part
        var rightPartFreeSpace = contentRectVisible.width - rightColumnsMinWidth;
        var cellExtraWidth = Mathf.Max(rightPartFreeSpace / rightColumnsCount, 0f);

        DrawPart(contentRectVisible, ScrollPosition, cellExtraWidth, false);

        Verse.Widgets.EndScrollView();
    }
    private void DrawPart(
        Rect rect,
        Vector2 scrollPosition,
        float cellExtraWidth,
        bool drawPinned
    )
    {
        DrawColumnSeparators(rect, Columns, scrollPosition.x, cellExtraWidth, drawPinned);

        var headersRect = rect.CutByY(TotalHeaderRowsHeight);

        DrawRows(headersRect, HeaderRows, scrollPosition with { y = 0f }, cellExtraWidth, drawPinned);
        // Register mouse-drag only below headers row to not interfere with filter inputs.
        DoHorScroll(rect, ref ScrollPosition);
        DrawRows(rect, BodyRows, scrollPosition, cellExtraWidth, drawPinned);
    }
    private static void DrawRows(
        Rect rect,
        IReadOnlyList<Row> rows,
        Vector2 scrollPosition,
        float cellExtraWidth,
        bool drawPinned
    )
    {
        GUI.BeginClip(rect);

        var yMax = rect.height;
        rect.x = 0f;
        rect.y = -scrollPosition.y;
        var i = 0;

        foreach (var row in rows)
        {
            if (rect.y >= yMax)
            {
                break;
            }

            if (row.IsVisible)
            {
                rect.height = row.Height;
                if (rect.yMax > 0f)
                {
                    row.Draw(rect, scrollPosition.x, drawPinned, cellExtraWidth, i);
                }

                rect.y = rect.yMax;
                i++;
            }
        }

        GUI.EndClip();
    }
    // The performance impact of instead drawing a vertical border for each
    // individual column's cell is huge. So we have to keep this.
    private static void DrawColumnSeparators(
        Rect rect,
        Column[] columns,
        float offsetX,
        float cellExtraWidth,
        bool drawPinned
    )
    {
        if (Event.current.type != EventType.Repaint)
        {
            return;
        }

        var x = -offsetX;

        foreach (var column in columns)
        {
            if (column.IsPinned != drawPinned)
            {
                continue;
            }

            x += column.Width + cellExtraWidth;
            if (x >= rect.width)
            {
                break;
            }

            if (x > 0f)
            {
                Widgets.Draw.VerticalLine(
                    x + rect.x - 1f,
                    rect.y,
                    rect.height,
                    ColumnSeparatorLineColor
                );
            }
        }
    }
    private static void DoHorScroll(Rect rect, ref Vector2 scrollPosition)
    {
        if (Event.current.type == EventType.MouseDrag && Mouse.IsOver(rect))
        {
            scrollPosition.x = Mathf.Max(scrollPosition.x + Event.current.delta.x * -1f, 0f);

            // Why no "Event.current.Use();"? Because the thing locks itself on mouse-up.
        }
    }
}
