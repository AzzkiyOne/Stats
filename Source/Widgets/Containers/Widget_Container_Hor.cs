using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stats;

public class Widget_Container_Hor
    : Widget
{
    private readonly float Gap;
    private readonly float ReservedSpaceAmount;
    private readonly List<IWidget> Children;
    public override Vector2 AbsSize { get; }
    public Widget_Container_Hor(List<IWidget> children, float gap = 0f)
    {
        Children = children;
        Gap = gap;

        var absSize = Vector2.zero;

        ReservedSpaceAmount = absSize.x = (children.Count - 1) * gap;

        foreach (var child in children)
        {
            ReservedSpaceAmount += child.GetSize(Vector2.zero).x;

            var childAbsSize = child.AbsSize;

            absSize.x += childAbsSize.x;
            absSize.y = Math.Max(absSize.y, childAbsSize.y);
        }

        AbsSize = absSize;
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        base.Draw(rect, containerSize);

        var rectSize = rect.size;
        rectSize.x -= ReservedSpaceAmount;

        foreach (var child in Children)
        {
            rect.size = child.GetSize(rectSize);
            child.Draw(rect, rectSize);

            rect.x = rect.xMax + Gap;
        }
    }
}
