using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stats;

public class Widget_Container_Ver
    : Widget
{
    private readonly float Gap;
    private readonly float ReservedSpaceAmount;
    private readonly List<IWidget> Children;
    public override Vector2 AbsSize { get; }
    public Widget_Container_Ver(List<IWidget> children, float gap = 0f)
    {
        Children = children;
        Gap = gap;

        var absSize = Vector2.zero;

        ReservedSpaceAmount = absSize.y = (children.Count - 1) * gap;

        foreach (var child in children)
        {
            ReservedSpaceAmount += child.GetSize(Vector2.zero).y;

            var childAbsSize = child.AbsSize;

            absSize.x = Math.Max(absSize.x, childAbsSize.x);
            absSize.y += childAbsSize.y;
        }

        AbsSize = absSize;
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        base.Draw(rect, containerSize);

        var rectSize = rect.size;
        rectSize.y -= ReservedSpaceAmount;

        foreach (var child in Children)
        {
            rect.size = child.GetSize(rectSize);
            child.Draw(rect, rectSize);

            rect.y = rect.yMax + Gap;
        }
    }
}
