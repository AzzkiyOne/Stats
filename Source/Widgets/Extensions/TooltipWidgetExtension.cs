using UnityEngine;
using Verse;

namespace Stats.Widgets.Extensions;

public sealed class TooltipWidgetExtension
    : WidgetExtension
{
    private readonly string Text;
    internal TooltipWidgetExtension(IWidget widget, string text)
        : base(widget)
    {
        Text = text;
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        TooltipHandler.TipRegion(rect, Text);

        Widget.Draw(rect, containerSize);
    }
}
