using UnityEngine;
using Verse;

namespace Stats.Widgets.Extensions;

public sealed class TooltipWidgetExtension : WidgetExtension
{
    private readonly TipSignal Tip;
    internal TooltipWidgetExtension(Widget widget, string tip) : base(widget)
    {
        Tip = tip;
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        TooltipHandler.TipRegion(rect, Tip);

        Widget.Draw(rect, containerSize);
    }
}
