using System.Collections.Generic;
using UnityEngine;

namespace Stats;

// See vertical variant for comments.
public class Widget_Container_Hor
    : Widget
{
    private readonly float Gap;
    private readonly List<IWidget> Children;
    private readonly float OccupiedSpaceAmount = 0f;
    public Widget_Container_Hor(
        List<IWidget> children,
        float gap = 0f,
        bool shareFreeSpace = false
    )
    {
        Children = children;
        Gap = gap;

        var totalGapAmount = (children.Count - 1) * gap;

        if (shareFreeSpace)
        {
            OccupiedSpaceAmount = totalGapAmount;
        }

        Vector2 size;
        size.x = totalGapAmount;
        size.y = 0f;

        foreach (var child in children)
        {
            var childSize = child.GetSize(Vector2.positiveInfinity);

            size.x += childSize.x;
            size.y = Mathf.Max(size.y, childSize.y);

            if (shareFreeSpace)
            {
                OccupiedSpaceAmount += child.GetSize(Vector2.zero).x;
            }
        }

        Size = size;
    }
    protected override void DrawContent(Rect rect)
    {
        var xMax = rect.xMax;
        var size = rect.size;
        size.x -= OccupiedSpaceAmount;

        if (WidthIsUndef) size.x = float.PositiveInfinity;
        if (HeightIsUndef) size.y = float.PositiveInfinity;

        foreach (var child in Children)
        {
            if (rect.x >= xMax) break;

            rect.size = child.GetSize(size);

            if (rect.xMax > 0f)
            {
                child.Draw(rect, size);
            }

            rect.x = rect.xMax + Gap;
        }
    }
}
