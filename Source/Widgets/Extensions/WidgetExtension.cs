using UnityEngine;

namespace Stats.Widgets.Extensions;

public abstract class WidgetExtension : Widget
{
    public sealed override Widget? Parent { set => Widget.Parent = value; }
    public Widget Widget { get; }
    public WidgetExtension(Widget widget)
    {
        Widget = widget;
    }
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
    public sealed override void UpdateSize()
    {
        Widget.UpdateSize();
    }
}
