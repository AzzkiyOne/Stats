using UnityEngine;

namespace Stats;

public abstract class Widget_Addon
    : IWidget
{
    protected IWidget Widget { get; }
    public WidgetStyle Style => Widget.Style;
    public Widget_Addon(IWidget widget)
    {
        Widget = widget;
    }
    public virtual Vector2 GetSize(in Vector2 containerSize)
    {
        return Widget.GetSize(containerSize);
    }
    public virtual Vector2 GetSize()
    {
        return Widget.GetSize();
    }
    public virtual void Draw(Rect rect, in Vector2 containerSize)
    {
        Widget.Draw(rect, containerSize);
    }
}
