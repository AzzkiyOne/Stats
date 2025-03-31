using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stats;

public class Widget_Container_Ver
    : Widget_Container
{
    private readonly float Gap;
    private readonly float TotalGapAmount;
    private readonly float ReservedSpaceAmount;
    public Widget_Container_Ver(
        List<IWidget> children,
        float gap = 0f,
        bool flex = false,
        WidgetStyle? style = null
    )
        : base(children, style)
    {
        Gap = gap;
        TotalGapAmount = (Children.Count - 1) * gap;
        ReservedSpaceAmount = TotalGapAmount;

        foreach (var child in Children)
        {
            if (child.Style.Height is WidgetStyle.Units.Abs or null)
            {
                ReservedSpaceAmount += child.GetSize().y;
            }
        }
    }
    protected override Vector2 CalcContentSize()
    {
        Vector2 result;

        result.x = 0f;
        result.y = TotalGapAmount;

        foreach (var child in Children)
        {
            var childSize = child.GetSize();

            result.x = Math.Max(result.x, childSize.x);
            result.y += childSize.y;
        }

        return result;
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        base.Draw(rect, containerSize);

        var childRect = new Rect(rect.position, Vector2.zero);
        var rectSize = rect.size;

        rectSize.y -= ReservedSpaceAmount;

        foreach (var child in Children)
        {
            childRect.size = child.GetSize(rectSize);

            child.Draw(childRect, rectSize);

            childRect.y = childRect.yMax + Gap;
        }
    }
}
