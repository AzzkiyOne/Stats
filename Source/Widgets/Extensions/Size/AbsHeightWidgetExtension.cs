using UnityEngine;

namespace Stats.Widgets.Extensions.Size;

public sealed class AbsHeightWidgetExtension : WidgetExtension
{
    private readonly float Height;
    internal AbsHeightWidgetExtension(Widget widget, float height) : base(widget)
    {
        Height = height;
    }
    public override Vector2 GetSize(Vector2 containerSize)
    {
        return Widget.GetSize(containerSize) with { y = Height };
    }
    public override Vector2 GetSize()
    {
        return Widget.GetSize() with { y = Height };
    }
}
