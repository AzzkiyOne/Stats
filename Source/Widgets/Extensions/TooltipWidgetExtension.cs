using UnityEngine;
using Verse;

namespace Stats.Widgets.Extensions;

public sealed class TooltipWidgetExtension
    : WidgetDecorator
{
    public override Widget Widget { get; }
    private readonly string Text;
    internal TooltipWidgetExtension(Widget widget, string text)
    {
        Widget = widget;
        Text = text;
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
