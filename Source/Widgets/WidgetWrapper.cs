using UnityEngine;

namespace Stats.Widgets;

public abstract class WidgetWrapper : Widget
{
    public sealed override Widget? Parent { set => Widget.Parent = value; }
    protected abstract Widget Widget { get; }
    public sealed override Vector2 GetSize(Vector2 containerSize)
    {
        return Widget.GetSize(containerSize);
    }
    public sealed override Vector2 GetSize()
    {
        return Widget.GetSize();
    }
    public sealed override void Draw(Rect rect, Vector2 containerSize)
    {
        Widget.Draw(rect, containerSize);
    }
    public sealed override void UpdateSize()
    {
        Widget.UpdateSize();
    }
}
