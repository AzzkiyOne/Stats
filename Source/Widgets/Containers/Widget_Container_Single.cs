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
    private readonly float OccupiedWidth = 0f;
    private readonly float OccupiedHeight = 0f;
    public Widget_Container_Single(IWidget widget)
    {
        Widget = widget;
        widget.Parent = this;

        var widgetSize = widget.GetSize(Vector2.zero);

        OccupiedWidth = widgetSize.x;
        OccupiedHeight = widgetSize.y;

        UpdateSize();
    }
    protected override Vector2 GetSize()
    {
        return Widget.GetSize(Vector2.positiveInfinity);
    }
    protected override void DrawContent(Rect rect)
    {
        var rectSize = rect.size;
        rectSize.x -= OccupiedWidth;
        rectSize.y -= OccupiedHeight;

        rect.size = Widget.GetSize(rectSize);
        Widget.Draw(rect, rectSize);
    }
}
