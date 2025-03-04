using UnityEngine;
using Verse;

namespace Stats;

internal class Widget_TableRow_Body
    : Widget_TableRow
{
    private bool IsHovered = false;
    protected override void Draw(Rect targetRect, int index)
    {
        if (Mouse.IsOver(targetRect))
        {
            IsHovered = true;
        }

        if (index % 2 == 0)
        {
            Widgets.DrawLightHighlight(targetRect);
        }

        if (IsHovered)
        {
            Widgets.DrawHighlight(targetRect);
        }

        if (Mouse.IsOver(targetRect) == false)
        {
            IsHovered = false;
        }
    }
}
