using UnityEngine;

namespace Stats.Widgets.Extensions;

public sealed class AbsHeightWidgetExtension : WidgetExtension
{
    private readonly float Height;
    internal AbsHeightWidgetExtension(Widget widget, float height) : base(widget)
    {
        Height = height;
    }
    protected override Vector2 CalcSize(Vector2 containerSize)
    {
        return Widget.GetSize(containerSize) with { y = Height };
    }
    protected override Vector2 CalcSize()
    {
        return Widget.GetSize() with { y = Height };
    }
}
