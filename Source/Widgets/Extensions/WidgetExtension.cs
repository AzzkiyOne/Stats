using UnityEngine;

namespace Stats.Widgets.Extensions;

public abstract class WidgetExtension : Widget
{
    public Widget Widget { get; }
    public WidgetExtension(Widget widget)
    {
        Widget = widget;
        widget.Parent = this;
    }
    protected override Vector2 CalcSize(Vector2 containerSize)
    {
        return Widget.GetSize(containerSize);
    }
    protected override Vector2 CalcSize()
    {
        return Widget.GetSize();
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        Widget.Draw(rect, containerSize);
    }
}
