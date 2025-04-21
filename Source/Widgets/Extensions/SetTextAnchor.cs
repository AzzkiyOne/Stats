using UnityEngine;
using Verse;

namespace Stats.Widgets.Extensions;

public sealed class SetTextAnchor
    : WidgetExtension
{
    private readonly TextAnchor Value;
    internal SetTextAnchor(IWidget widget, TextAnchor value)
        : base(widget)
    {
        Value = value;
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        var origTextAnchor = Text.Anchor;
        Text.Anchor = Value;

        Widget.Draw(rect, containerSize);

        Text.Anchor = origTextAnchor;
    }
}

public static partial class WidgetAPI
{
    public static SetTextAnchor TextAnchor(this IWidget widget, TextAnchor value)
    {
        return new(widget, value);
    }
}
