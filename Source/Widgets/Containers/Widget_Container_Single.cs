using UnityEngine;

namespace Stats.Widgets.Containers;

public class Widget_Container_Single
    : Widget
{
    protected override Vector2 Size { get; set; }
    private readonly IWidget Widget;
    private readonly float OccupiedWidth = 0f;
    private readonly float OccupiedHeight = 0f;
    public Widget_Container_Single(IWidget widget)
    {
        Widget = widget;
        widget.Parent = this;

        var widgetSize = widget.GetFixedSize();

        OccupiedWidth = widgetSize.x;
        OccupiedHeight = widgetSize.y;
        Size = GetSize();
    }
    public override Vector2 GetSize()
    {
        return Widget.GetSize();
    }
    protected override void DrawContent(Rect rect)
    {
        var size = rect.size;
        size.x = Mathf.Max(size.x - OccupiedWidth, 0f);
        size.y = Mathf.Max(size.y - OccupiedHeight, 0f);

        rect.size = Widget.GetSize(size);
        Widget.Draw(rect, size);
    }
}
