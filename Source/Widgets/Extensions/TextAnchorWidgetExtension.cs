using UnityEngine;
using Verse;

namespace Stats.Widgets.Extensions;

public sealed class TextAnchorWidgetExtension
    : WidgetDecorator
{
    public override Widget Widget { get; }
    private readonly TextAnchor Value;
    internal TextAnchorWidgetExtension(Widget widget, TextAnchor value)
    {
        Widget = widget;
        Value = value;
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        var origTextAnchor = Text.Anchor;
        Text.Anchor = Value;

        Widget.Draw(rect, containerSize);

        Text.Anchor = origTextAnchor;
    }
}
