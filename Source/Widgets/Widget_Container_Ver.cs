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
                    ReservedSpaceAmount += child.GetMarginBoxSize().y;
                }
            }
        }
    }
    public override Vector2 CalcContentSize()
    {
        Vector2 result;

        result.x = 0f;
        result.y = TotalGapAmount;

        foreach (var child in Children)
        {
            var childMarginBoxSize = child.GetMarginBoxSize();

            result.x = Math.Max(result.x, childMarginBoxSize.x);
            result.y += childMarginBoxSize.y;
        }

        return result;
    }
    public override void DrawContentBox(Rect contentBox)
    {
        var childMarginBox = new Rect(contentBox.position, Vector2.zero);
        var contentBoxSize = contentBox.size;

        contentBoxSize.y -= ReservedSpaceAmount;

        foreach (var child in Children)
        {
            childMarginBox.size = child.GetMarginBoxSize(contentBoxSize);

            child.DrawMarginBoxIn(childMarginBox, contentBox);

            childMarginBox.y = childMarginBox.yMax + Gap;
        }
    }
}
