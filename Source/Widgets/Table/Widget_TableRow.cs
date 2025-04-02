using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using Verse;

namespace Stats;

internal class Widget_TableRow
{
    private readonly List<Widget_TableCell> _Cells = [];
    public ReadOnlyCollection<Widget_TableCell> Cells => _Cells.AsReadOnly();
    public float Height = 0f;
    private readonly OnDraw DrawBG;
    private bool IsHovered = false;
    public Widget_TableRow(OnDraw onDraw)
    {
        DrawBG = onDraw;
    }
    public void AddCell(Widget_TableCell cell)
    {
        var cellAbsSize = cell.AbsSize;

        cell.Column.Width = Math.Max(cell.Column.Width, cellAbsSize.x);
        Height = Math.Max(Height, cellAbsSize.y);

        _Cells.Add(cell);
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

        foreach (var cell in _Cells)
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

    internal delegate void OnDraw(ref Rect rect, in bool isHovered, in int index);
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
