using UnityEngine;

namespace Stats;

public class Widget_Addon_Margin
    : Widget_Addon_RectOffset
{
    public Widget_Addon_Margin(IWidget widget, float hor, float ver)
        : base(widget, hor, ver)
    {
    }
    public Widget_Addon_Margin(IWidget widget, float l, float r, float t, float b)
        : base(widget, l, r, t, b)
    {
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        Vector2 result = Widget.GetSize(containerSize);

        result.x += Hor;
        result.y += Ver;

        return result;
    }
    public override Vector2 GetSize()
    {
        Vector2 result = Widget.GetSize();

        result.x += Hor;
        result.y += Ver;

        return result;
    }
}
