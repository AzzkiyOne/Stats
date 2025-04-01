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
    private readonly Vector2 _ContentSize = Vector2.zero;
    protected override Vector2 ContentSize => _ContentSize;
    public Widget_Container_Hor(List<IWidget> children, float gap = 0f)
    {
        Children = children;
        Gap = gap;
        ReservedSpaceAmount = _ContentSize.x = (children.Count - 1) * gap;

        foreach (var child in children)
        {
            ReservedSpaceAmount += child.GetSize(Vector2.zero).x;

            var childSize = child.GetSize();

            _ContentSize.x += childSize.x;
            _ContentSize.y = Math.Max(_ContentSize.y, childSize.y);
        }
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
