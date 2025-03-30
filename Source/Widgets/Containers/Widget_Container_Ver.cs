using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stats;

public class Widget_Container_Ver
    : Widget_Container_XY
{
    public Widget_Container_Ver(
        List<Widget> children,
        float gap = 0f,
        bool flex = false,
        WidgetStyle? style = null
    )
        : base(children, gap, style)
    {
        if (flex)
        {
            foreach (var child in Children)
            {
                if (child.Style.Height is WidgetStyle.Units.Abs or null)
                {
                    ReservedSpaceAmount += child.GetSize().y;
                }
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
    public override void Draw(Rect rect)
    {
        base.Draw(rect);

        var childRect = new Rect(rect.position, Vector2.zero);
        var rectSize = rect.size;

        rectSize.y -= ReservedSpaceAmount;

        foreach (var child in Children)
        {
            childRect.size = child.GetSize(rectSize);

            child.Draw(childRect);

            childRect.y = childRect.yMax + Gap;
        }
    }
}
