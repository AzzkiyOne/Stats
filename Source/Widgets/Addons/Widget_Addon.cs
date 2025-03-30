using UnityEngine;

namespace Stats;

public abstract class Widget_Addon
    : Widget
{
    protected Widget Widget { get; }
    public override WidgetStyle Style => Widget.Style;
    public Widget_Addon(Widget widget)
    {
        Widget = widget;
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        return Widget.GetSize(containerSize);
    }
    public override Vector2 GetSize()
    {
        return Widget.GetSize();
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        Widget.Draw(rect, containerSize);
    }
}
