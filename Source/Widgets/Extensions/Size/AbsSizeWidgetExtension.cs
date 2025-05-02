using UnityEngine;

namespace Stats.Widgets.Extensions.Size;

public sealed class AbsSizeWidgetExtension : WidgetExtension
{
    private readonly Vector2 Size;
    internal AbsSizeWidgetExtension(Widget widget, float width, float height) : base(widget)
    {
        Size.x = width;
        Size.y = height;
    }
    protected override Vector2 CalcSize(Vector2 _)
    {
        return Size;
    }
    protected override Vector2 CalcSize()
    {
        return Size;
    }
}
