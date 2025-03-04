using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stats;

internal abstract class Widget_TableRow
{
    public List<IWidget_TableCell> Cells { get; } = [];
    public Widget_TableRow()
    {
    }
    public void Draw(
        Rect targetRect,
        float offsetX,
        Func<IWidget_TableCell, bool> shouldDrawCell,
        float cellExtraWidth,
        int index
    )
    {
        Draw(targetRect, index);

        // Cells
        var x = -offsetX;

        foreach (var cell in Cells)
        {
            if (shouldDrawCell(cell) == false) continue;
            if (x >= targetRect.width) break;

            var cellWidth = cell.Width + cellExtraWidth;
            var xMax = x + cellWidth;

            if (xMax > 0f)
            {
                var cellRect = new Rect(
                    x,
                    targetRect.y,
                    cellWidth,
                    targetRect.height
                );

                cell.Draw(cellRect);
            }

            x = xMax;
        }
    }
    protected abstract void Draw(Rect targetRect, int index);
}
