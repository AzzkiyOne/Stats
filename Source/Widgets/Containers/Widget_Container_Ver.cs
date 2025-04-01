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
    private readonly Vector2 _ContentSize = Vector2.zero;
    protected override Vector2 ContentSize => _ContentSize;
    public Widget_Container_Ver(List<IWidget> children, float gap = 0f)
    {
        Children = children;
        Gap = gap;
        ReservedSpaceAmount = _ContentSize.y = (children.Count - 1) * gap;

        foreach (var child in children)
        {
            ReservedSpaceAmount += child.GetSize(Vector2.zero).y;

            var childSize = child.GetSize();

            _ContentSize.x = Math.Max(_ContentSize.x, childSize.x);
            _ContentSize.y += childSize.y;
        }
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
