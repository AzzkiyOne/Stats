using UnityEngine;
using Verse;

namespace Stats.Widgets.Comps;

public class TooltipWidgetComp
    : WidgetComp
{
    private readonly string Text;
    public TooltipWidgetComp(ref IWidget widget, string text)
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
