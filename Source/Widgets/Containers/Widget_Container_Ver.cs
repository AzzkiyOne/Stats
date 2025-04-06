using System.Collections.Generic;
using UnityEngine;

namespace Stats;

public class Widget_Container_Ver
    : Widget
{
    private readonly float Gap;
    private readonly List<IWidget> Children;
    private readonly bool ShareFreeSpace;
    private float OccupiedSpaceAmount = 0f;
    public Widget_Container_Ver(
        List<IWidget> children,
        float gap = 0f,
        bool shareFreeSpace = false
    )
    {
        Children = children;
        Gap = gap;
        ShareFreeSpace = shareFreeSpace;

        foreach (var child in children)
        {
            child.Parent = this;
        }

        UpdateSize();
    }
    protected override Vector2 GetSize()
    {
        OccupiedSpaceAmount = 0f;

        var totalGapAmount = (Children.Count - 1) * Gap;

        if (ShareFreeSpace)
        {
            OccupiedSpaceAmount = totalGapAmount;
        }

        Vector2 size;
        size.x = 0f;
        size.y = totalGapAmount;

        foreach (var child in Children)
        {
            var childSize = child.GetSize(Vector2.positiveInfinity);

            size.x = Mathf.Max(size.x, childSize.x);
            size.y += childSize.y;

            if (ShareFreeSpace)
            {
                OccupiedSpaceAmount += child.GetSize(Vector2.zero).y;
            }
        }

        return size;
    }
    protected override void DrawContent(Rect rect)
    {
        var yMax = rect.yMax;
        var size = rect.size;
        size.y -= OccupiedSpaceAmount;

        // If container's size is undefined, its children have no base to calculate
        // their relative dimensions from.
        if (WidthIsUndef) size.x = float.PositiveInfinity;
        if (HeightIsUndef) size.y = float.PositiveInfinity;

        foreach (var child in Children)
        {
            if (rect.y >= yMax) break;

            rect.size = child.GetSize(size);

            // This is for (future) scroll component.
            if (rect.yMax > 0f)
            {
                child.Draw(rect, size);
            }

            rect.y = rect.yMax + Gap;
        }
    }
}
