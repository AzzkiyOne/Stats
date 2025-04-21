using UnityEngine;

namespace Stats.Widgets.Comps;

public class HideOverflow
    : WidgetComp
{
    public HideOverflow(ref IWidget widget)
        : base(ref widget)
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
