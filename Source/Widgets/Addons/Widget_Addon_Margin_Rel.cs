using UnityEngine;

namespace Stats;

public class Widget_Addon_Margin_Rel
    : Widget_Addon
{
    private readonly WidgetStyle.Units.Unit Left;
    private readonly WidgetStyle.Units.Unit Right;
    private readonly WidgetStyle.Units.Unit Top;
    private readonly WidgetStyle.Units.Unit Bottom;
    public Widget_Addon_Margin_Rel(
        IWidget widget,
        WidgetStyle.Units.Unit l,
        WidgetStyle.Units.Unit r,
        WidgetStyle.Units.Unit t,
        WidgetStyle.Units.Unit b
    )
        : base(widget)
    {
        Left = l;
        Right = r;
        Top = t;
        Bottom = b;
    }
    public Widget_Addon_Margin_Rel(
        IWidget widget,
        WidgetStyle.Units.Unit hor,
        WidgetStyle.Units.Unit ver
    )
        : this(widget, hor, hor, ver, ver)
    {
    }
    private float Hor(float value)
    {
        return Left.Get(value) + Right.Get(value);
    }
    private float Ver(float value)
    {
        return Top.Get(value) + Bottom.Get(value);
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        Vector2 result = Widget.GetSize(containerSize);

        result.x += Hor(containerSize.x);
        result.y += Ver(containerSize.y);

        return result;
    }
    public override Vector2 GetSize()
    {
        Vector2 result = Widget.GetSize();

        result.x += Hor(0f);
        result.y += Ver(0f);

        return result;
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        rect.x += Left.Get(containerSize.x);
        rect.y += Top.Get(containerSize.y);
        rect.width -= Hor(containerSize.x);
        rect.height -= Ver(containerSize.y);

        Widget.Draw(rect, containerSize);
    }
}
