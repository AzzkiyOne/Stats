using UnityEngine;
using Verse;

namespace Stats;

public class Widget_Addon_Tooltip
    : Widget_Addon
{
    private readonly string Text;
    public Widget_Addon_Tooltip(Widget widget, string text)
        : base(widget)
    {
        Text = text;
    }
    public override void Draw(Rect rect)
    {
        TooltipHandler.TipRegion(rect, Text);

        Widget.Draw(rect);
    }
}
