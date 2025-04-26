using UnityEngine;
using Verse;

namespace Stats.Widgets.Extensions;

public sealed class TooltipWidgetExtension : WidgetExtension
{
    private readonly string Text;
    internal TooltipWidgetExtension(Widget widget, string text) : base(widget)
    {
        Text = text;

        Resize();
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        // Although this is a duplicate check, things should be faster on
        // average, because TooltipHandler.TipRegion takes TipSignal as an
        // argument, which has an implicit conversion from string.
        if (Mouse.IsOver(rect))
        {
            TooltipHandler.TipRegion(rect, Text);
        }

        Widget.Draw(rect, containerSize);
    }
}
