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
    private float _Height = 0f;
    public float Height
    {
        get => _Height;
        set => _Height = Math.Max(_Height, value);
    }
    public Action<Rect, bool, int>? Background { get; set; }
    private bool IsHovered = false;
    public Widget_TableRow()
    {
    }
    public void AddCell(Widget_TableCell cell)
    {
        var cellSize = cell.GetMarginBoxSize();

        cell.Props.Width = cellSize.x;
        Height = cellSize.y;

        _Cells.Add(cell);
    }
    public void Draw(
        Rect targetRect,
        in float offsetX,
        Func<Widget_TableCell, bool> shouldDrawCell,
        in float cellExtraWidth,
        in int index
    )
    {
        if (Mouse.IsOver(targetRect))
        {
            IsHovered = true;
        }

        Background?.Invoke(targetRect, IsHovered, index);

        if (Mouse.IsOver(targetRect) == false)
        {
            IsHovered = false;
        }

        // Cells
        var x = -offsetX;

        foreach (var cell in _Cells)
        {
            if (shouldDrawCell(cell) == false) continue;
            if (x >= targetRect.width) break;

            var cellWidth = cell.Props.Width + cellExtraWidth;
            var xMax = x + cellWidth;

            if (xMax > 0f && cell.IsEmpty == false)
            {
                var cellRect = new Rect(
                    x,
                    targetRect.y,
                    cellWidth,
                    targetRect.height
                );

                cell.DrawMarginBox(cellRect);
            }

            x = xMax;
        }
    }
}

internal sealed class Widget_TableRow<IdType>
        : Widget_TableRow
{
    public required IdType Id { get; init; }
}
