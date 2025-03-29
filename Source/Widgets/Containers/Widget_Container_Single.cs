using UnityEngine;

namespace Stats;

// This is mainly for wrapping non-containers to apply padding.
// 
// For example, Widget_Icon has content size of 0. Although we can probably
// measure a texture, using its size as content size would be unpractical.
public class Widget_Container_Single
    : Widget_Drawable
{
    private readonly Widget Widget;
    protected override Vector2 ContentSize { get; }
    public Widget_Container_Single(Widget widget, WidgetStyle? style = null)
        : base(style)
    {
        Widget = widget;
        ContentSize = widget.GetSize();
    }
    public override void Draw(Rect rect)
    {
        base.Draw(rect);

        Widget.DrawIn(rect);
    }
}
