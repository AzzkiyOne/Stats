using UnityEngine;

namespace Stats.Widgets;

public abstract class WidgetWrapper : Widget
{
    protected abstract Widget Widget { get; }
    protected sealed override Vector2 CalcSize(Vector2 containerSize)
    {
        return Widget.GetSize(containerSize);
    }
    protected sealed override Vector2 CalcSize()
    {
        return Widget.GetSize();
    }
    public sealed override void Draw(Rect rect, Vector2 containerSize)
    {
        Widget.Draw(rect, containerSize);
    }
}
