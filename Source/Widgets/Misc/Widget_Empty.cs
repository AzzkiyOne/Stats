using UnityEngine;

namespace Stats;

public class Widget_Empty
    : Widget_Drawable
{
    protected override Vector2 ContentSize { get; } = Vector2.zero;
    public Widget_Empty(WidgetStyle? style = null)
        : base(style)
    {
    }
}
