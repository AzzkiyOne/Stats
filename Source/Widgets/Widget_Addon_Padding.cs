using UnityEngine;

namespace Stats;

// Should only be applied to containers.
public class Widget_Addon_Padding
    : Widget_Addon_RectOffset
{
    public Widget_Addon_Padding(Widget widget, (float hor, float ver) vals)
        : base(widget, vals)
    {
    }
    public Widget_Addon_Padding(Widget widget, float l, float r, float t, float b)
        : base(widget, l, r, t, b)
    {
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        var size = Widget.GetSize(containerSize);

        if (Widget.Style.Width == null)
        {
            size.x += Hor;
        }

        if (Widget.Style.Height == null)
        {
            size.y += Ver;
        }

        return size;
    }
    public override Vector2 GetSize()
    {
        var size = Widget.GetSize();

        if (Widget.Style.Width is not WidgetStyle.Units.Abs)
        {
            size.x += Hor;
        }

        if (Widget.Style.Height is not WidgetStyle.Units.Abs)
        {
            size.y += Ver;
        }

        return size;
    }
}
