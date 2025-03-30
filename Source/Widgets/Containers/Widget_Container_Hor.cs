using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stats;

public class Widget_Container_Hor
    : Widget_Container_XY
{
    public Widget_Container_Hor(
        List<Widget> children,
        float gap = 0f,
        WidgetStyle? style = null
    )
        : base(children, gap, style)
    {
        foreach (var child in Children)
        {
            if (child.Style.Width is WidgetStyle.Units.Abs or null)
            {
                ReservedSpaceAmount += child.GetSize().x;
            }
        }
    }
    protected override Vector2 CalcContentSize()
    {
        Vector2 result;

        result.x = TotalGapAmount;
        result.y = 0f;

        foreach (var child in Children)
        {
            var childSize = child.GetSize();

            result.x += childSize.x;
            result.y = Math.Max(result.y, childSize.y);
        }

        return result;
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        base.Draw(rect, containerSize);

        var childRect = new Rect(rect.position, Vector2.zero);
        var rectSize = rect.size;

        rectSize.x -= ReservedSpaceAmount;

        foreach (var child in Children)
        {
            childRect.size = child.GetSize(rectSize);

            child.Draw(childRect, rectSize);

            childRect.x = childRect.xMax + Gap;
        }
    }
}
