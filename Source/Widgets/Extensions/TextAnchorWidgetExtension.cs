using UnityEngine;
using Verse;

namespace Stats.Widgets.Extensions;

public sealed class TextAnchorWidgetExtension
    : WidgetExtension
{
    private readonly TextAnchor Value;
    internal TextAnchorWidgetExtension(IWidget widget, TextAnchor value)
        : base(widget)
    {
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
