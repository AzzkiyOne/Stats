using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats;

internal class Widget_TableRow
{
    public List<WidgetComp_TableCell> Cells { get; } = [];
    public float Height = 0f;
    public bool IsHidden = false;
    private readonly OnDraw DrawBG;
    private bool IsHovered = false;
    public Widget_TableRow(OnDraw onDraw)
    {
        DrawBG = onDraw;
    }
    public void Draw(
        Rect rect,
        in float offsetX,
        in bool drawPinned,
        in float cellExtraWidth,
        in int index
    )
    {
        if (Mouse.IsOver(rect))
        {
            IsHovered = true;
        }

        DrawBG(ref rect, IsHovered, index);

        if (Mouse.IsOver(rect) == false)
        {
            IsHovered = false;
        }

        // Cells
        var cellRect = new Rect(-offsetX, rect.y, 0f, rect.height);

        foreach (var cell in Cells)
        {
            if (cell.Column.IsPinned != drawPinned) continue;
            if (cellRect.x >= rect.width) break;

            cellRect.width = cell.Column.Width + cellExtraWidth;

            if (cellRect.xMax > 0f)
            {
                cell.DrawIn(cellRect);
            }

            cellRect.x = cellRect.xMax;
        }
    }

    internal delegate void OnDraw(ref Rect rect, bool isHovered, int index);
}

internal sealed class Widget_TableRow<IdType>
        : Widget_TableRow
{
    public IdType Id { get; }
    public Widget_TableRow(OnDraw onDraw, IdType id)
        : base(onDraw)
    {
        Id = id;
    }
}
