using UnityEngine;
using Verse;

namespace Stats.Widgets.Extensions;

public sealed class DrawTooltipOnHover
    : WidgetExtension
{
    private readonly string Text;
    internal DrawTooltipOnHover(IWidget widget, string text)
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

public static partial class WidgetAPI
{
    public static DrawTooltipOnHover Tooltip(this IWidget widget, string text)
    {
        return new(widget, text);
    }
}
