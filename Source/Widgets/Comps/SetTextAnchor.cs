using UnityEngine;
using Verse;

namespace Stats.Widgets.Comps;

public class SetTextAnchor
    : WidgetComp
{
    private readonly TextAnchor Value;
    public SetTextAnchor(ref IWidget widget, TextAnchor value)
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
