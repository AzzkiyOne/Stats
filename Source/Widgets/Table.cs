using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

internal sealed class Table
{
    public List<Column> Columns { get; }
    private readonly List<TableRow> HeaderRows;
    public List<TableRow> BodyRows { get; }
    private float TotalHeaderRowsHeight = 0f;
    private float TotalBodyRowsHeight = 0f;
    private Vector2 ScrollPos = new();
    private static Color ColumnSeparatorLineColor = new(1f, 1f, 1f, 0.05f);
    private bool ShouldRecalcLayout = false;
    public Table(
        List<Column> columns,
        List<TableRow> headerRows,
        List<TableRow> bodyRows
    )
    {
        Columns = columns;
        HeaderRows = headerRows;
        BodyRows = bodyRows;

        AttachTo(headerRows);
        AttachTo(bodyRows);
        RecalcLayout();
    }
    public void Draw(Rect rect)
    {
        if (ShouldRecalcLayout && Event.current.type == EventType.Layout)
        {
            TEMP_RecalcOnlyHeaders();
            ShouldRecalcLayout = false;
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

        Verse.Widgets.BeginScrollView(rect, ref ScrollPos, contentRectMax, true);

        var contentRectVisible = new Rect(ScrollPos, contentSizeVisible);

        // Left part
        if (leftColumnsMinWidth > 0f)
        {
            var leftPartRect = contentRectVisible.CutByX(leftColumnsMinWidth);

            DrawPart(leftPartRect, ScrollPos with { x = 0f }, 0f, true);

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

        DrawPart(contentRectVisible, ScrollPos, cellExtraWidth, false);

        Verse.Widgets.EndScrollView();
    }
    private void DrawPart(
        Rect rect,
        Vector2 scrollPos,
        float cellExtraWidth,
        bool drawPinned
    )
    {
        DrawColumnSeparators(rect, Columns, scrollPos.x, cellExtraWidth, drawPinned);

        var headersRect = rect.CutByY(TotalHeaderRowsHeight);

        DrawRows(headersRect, HeaderRows, scrollPos with { y = 0f }, cellExtraWidth, drawPinned);
        // Register mouse-drag only below headers row to not interfere with filter inputs.
        DoHorScroll(rect);
        DrawRows(rect, BodyRows, scrollPos, cellExtraWidth, drawPinned);
    }
    private static void DrawRows(
        Rect rect,
        List<TableRow> rows,
        Vector2 scrollPos,
        float cellExtraWidth,
        bool drawPinned
    )
    {
        GUI.BeginClip(rect);

        var yMax = rect.height;
        rect.x = 0f;
        rect.y = -scrollPos.y;
        var i = 0;

        foreach (var row in rows)
        {
            if (rect.y >= yMax)
            {
                break;
            }

            if (row.IsVisible == false)
            {
                continue;
            }

            rect.height = row.Height;
            if (rect.yMax > 0f)
            {
                row.Draw(rect, scrollPos.x, drawPinned, cellExtraWidth, i);
            }

            rect.y = rect.yMax;
            i++;
        }

        GUI.EndClip();
    }
    // The performance impact of instead drawing a vertical border for each
    // individual column's cell is huge. So we have to keep this.
    private static void DrawColumnSeparators(
        Rect rect,
        List<Column> columns,
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
    private void DoHorScroll(Rect rect)
    {
        if (Event.current.type == EventType.MouseDrag && Mouse.IsOver(rect))
        {
            ScrollPos.x = Mathf.Max(ScrollPos.x + Event.current.delta.x * -1f, 0f);

            // Why no "Event.current.Use();"? Because the thing locks itself on mouse-up.
        }
    }
    public void ScheduleLayoutRecalc()
    {
        ShouldRecalcLayout = true;
    }
    private void TEMP_RecalcOnlyHeaders()
    {
        foreach (var column in Columns)
        {
            column.Width = 0f;
        }

        TotalHeaderRowsHeight = 0f;
        TotalBodyRowsHeight = 0f;

        foreach (var row in HeaderRows)
        {
            if (row.IsVisible)
            {
                row.TEMP_RecalcLayout();
                TotalHeaderRowsHeight += row.Height;
            }
        }

        foreach (var row in BodyRows)
        {
            if (row.IsVisible)
            {
                TotalBodyRowsHeight += row.Height;
            }
        }
    }
    // TODO: Could it be faster to recalc by column?
    private void RecalcLayout()
    {
        foreach (var column in Columns)
        {
            column.Width = 0f;
        }

        //ScrollPos.y = 0f;
        TotalHeaderRowsHeight = RecalcRows(HeaderRows);
        TotalBodyRowsHeight = RecalcRows(BodyRows);
    }
    private static float RecalcRows(List<TableRow> rows)
    {
        var totalHeight = 0f;

        foreach (var row in rows)
        {
            if (row.IsVisible)
            {
                row.RecalcLayout();
                totalHeight += row.Height;
            }
        }

        return totalHeight;
    }
    private void AttachTo(List<TableRow> rows)
    {
        foreach (var row in rows)
        {
            row.Parent = this;
        }
    }

    public class Column
    {
        public bool IsPinned;
        public float Width;
        public float InitialWidth;
        public readonly TextAnchor TextAnchor;
        public Column(bool isPinned, TextAnchor textAnchor)
        {
            IsPinned = isPinned;
            TextAnchor = textAnchor;
        }
    }
}
