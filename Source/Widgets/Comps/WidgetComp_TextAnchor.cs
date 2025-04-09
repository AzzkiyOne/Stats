using UnityEngine;
using Verse;

namespace Stats;

public class WidgetComp_TextAnchor
    : WidgetComp
{
    private readonly TextAnchor Value;
    public WidgetComp_TextAnchor(ref IWidget widget, TextAnchor value)
        : base(ref widget)
    {
        Value = value;
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        Text.Anchor = Value;

        Widget.Draw(rect, containerSize);

        Text.Anchor = Constants.DefaultTextAnchor;
    }
}
