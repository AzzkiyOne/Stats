using UnityEngine;

namespace Stats;

public class WidgetComp_Overflow_Hide
    : WidgetComp
{
    public WidgetComp_Overflow_Hide(IWidget widget)
        : base(widget)
    {
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        GUI.BeginClip(rect);

        rect.x = 0f;
        rect.y = 0f;
        Widget.Draw(rect, containerSize);

        GUI.EndClip();
    }
}
