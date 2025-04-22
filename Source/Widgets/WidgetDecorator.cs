using UnityEngine;

namespace Stats.Widgets;

public abstract class WidgetDecorator
    : Widget
{
    public override Widget? Parent { set => Widget.Parent = value; }
    public abstract Widget Widget { get; }
    public override Vector2 GetSize(Vector2 containerSize)
    {
        return Widget.GetSize(containerSize);
    }
    public override Vector2 GetSize()
    {
        return Widget.GetSize();
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        Widget.Draw(rect, containerSize);
    }
    public override void UpdateSize()
    {
        Widget.UpdateSize();
    }
}
