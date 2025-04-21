using UnityEngine;

namespace Stats.Widgets;

public abstract class WidgetDecorator
    : IWidget
{
    public IWidget? Parent { set => Widget.Parent = value; }
    public abstract IWidget Widget { get; }
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
    public void UpdateSize()
    {
        Widget.UpdateSize();
    }
}
