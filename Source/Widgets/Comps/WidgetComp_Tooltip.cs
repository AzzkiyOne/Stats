using UnityEngine;
using Verse;

namespace Stats.Widgets.Comps;

public class WidgetComp_Tooltip
    : WidgetComp
{
    private readonly string Text;
    public WidgetComp_Tooltip(ref IWidget widget, string text)
        : base(ref widget)
    {
        Text = text;
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        TooltipHandler.TipRegion(rect, Text);

        Widget.Draw(rect, containerSize);
    }
}
