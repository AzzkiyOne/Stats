using UnityEngine;

namespace Stats;

// This is mainly for wrapping non-containers to apply padding.
// 
// For example, Widget_Icon has content size of 0. Although we can probably
// measure a texture, using its size as content size would be unpractical.
public class Widget_Container_Single
    : Widget_Drawable
{
    private readonly IWidget Widget;
    protected override Vector2 ContentSize { get; }
    private readonly float ReservedWidth = 0f;
    private readonly float ReservedHeight = 0f;
    public Widget_Container_Single(IWidget widget, WidgetStyle? style = null)
        : base(style)
    {
        Widget = widget;
        ContentSize = widget.GetSize();

        if (widget.Style.Width is WidgetStyle.Units.Abs or null)
        {
            ReservedWidth = ContentSize.x;
        }

        if (widget.Style.Height is WidgetStyle.Units.Abs or null)
        {
            ReservedHeight = ContentSize.y;
        }
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        base.Draw(rect, containerSize);

        var rectSize = rect.size;

        rectSize.x -= ReservedWidth;
        rectSize.y -= ReservedHeight;

        Widget.Draw(new Rect(rect.position, Widget.GetSize(rectSize)), rectSize);
    }
}
