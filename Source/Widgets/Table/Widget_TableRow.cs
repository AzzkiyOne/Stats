using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using Verse;

namespace Stats;

internal class Widget_TableRow
{
    private readonly List<IWidget_TableCell> _Cells = [];
    public ReadOnlyCollection<IWidget_TableCell> Cells => _Cells.AsReadOnly();
    private float _Height = 0f;
    public float Height
    {
        get => _Height;
        set => _Height = Math.Max(_Height, value);
    }
    private readonly OnDraw DrawBG;
    private bool IsHovered = false;
    public Widget_TableRow(OnDraw onDraw)
    {
        DrawBG = onDraw;
    }
    public void AddCell(IWidget_TableCell cell)
    {
        var cellSize = cell.GetSize();

        cell.Column.Width = cellSize.x;
        Height = cellSize.y;

        _Cells.Add(cell);
    }
    public void Draw(
        Rect rect,
        in float offsetX,
        Func<IWidget_TableCell, bool> shouldDrawCell,
        in float cellExtraWidth,
        in int index
    )
    {
        if (Mouse.IsOver(rect))
        {
            IsHovered = true;
        }

        DrawBG(rect, IsHovered, index);

        if (Mouse.IsOver(rect) == false)
        {
            IsHovered = false;
        }

        // Cells
        var cellRect = new Rect(-offsetX, rect.y, 0f, rect.height);

        foreach (var cell in _Cells)
        {
            if (shouldDrawCell(cell) == false) continue;
            if (cellRect.x >= rect.width) break;

            cellRect.width = cell.Column.Width + cellExtraWidth;

            if (cellRect.xMax > 0f)
            {
                cell.DrawIn(cellRect);
            }

            cellRect.x = cellRect.xMax;
        }
    }

    internal delegate void OnDraw(Rect rect, bool isHovered, int index);
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
