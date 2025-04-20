using UnityEngine;
using Verse;

namespace Stats.Widgets.Comps;

public class TextAnchorWidgetComp
    : WidgetComp
{
    private readonly TextAnchor Value;
    public TextAnchorWidgetComp(ref IWidget widget, TextAnchor value)
        : base(ref widget)
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
