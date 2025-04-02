using UnityEngine;

namespace Stats;

// This is mainly for wrapping non-containers to apply padding.
// 
// For example, Widget_Icon has content size of 0. Although we can probably
// measure a texture, using its size as content size would be unpractical.
public class Widget_Container_Single
    : Widget
{
    private readonly IWidget Widget;
    private readonly float ReservedWidth = 0f;
    private readonly float ReservedHeight = 0f;
    public override Vector2 AbsSize { get; }
    public Widget_Container_Single(IWidget widget)
    {
        Widget = widget;
        AbsSize = widget.AbsSize;

        var widgetSize = widget.GetSize(Vector2.zero);

        ReservedWidth = widgetSize.x;
        ReservedHeight = widgetSize.y;
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        base.Draw(rect, containerSize);

        var rectSize = rect.size;
        rectSize.x -= ReservedWidth;
        rectSize.y -= ReservedHeight;

        rect.size = Widget.GetSize(rectSize);
        Widget.Draw(rect, rectSize);
    }
}
