using UnityEngine;
using Verse;

namespace Stats;

public class Widget_Tooltip
    : Widget_Proxy
{
    private readonly string Text;
    public Widget_Tooltip(Widget widget, string text)
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
