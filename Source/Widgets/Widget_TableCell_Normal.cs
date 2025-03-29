using UnityEngine;

namespace Stats;

internal sealed class Widget_TableCell_Normal
    : Widget_TableCell
{
    private readonly Widget Widget;
    public override WidgetStyle Style => Widget.Style;
    public Widget_TableCell_Normal(
        Widget widget,
        Properties props
    )
        : base(props)
    {
        Widget = widget;
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        return Widget.GetSize(containerSize);
    }
    public override Vector2 GetSize()
    {
        return Widget.GetSize();
    }
    public override void Draw(Rect rect)
    {
        Widget.Draw(rect);
    }
}
