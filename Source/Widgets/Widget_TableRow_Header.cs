using UnityEngine;
using Verse;

namespace Stats;

internal class Widget_TableRow_Header
    : Widget_TableRow
{
    protected override void Draw(Rect targetRect, int index)
    {
        Widgets.DrawHighlight(targetRect);
        Widgets.DrawLineHorizontal(
            targetRect.x,
            targetRect.yMax - 1f,
            targetRect.width,
            StatsMainTabWindow.BorderLineColor
        );
    }
}
