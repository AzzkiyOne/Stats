using System.Collections.Generic;
using UnityEngine;

namespace Stats.Widgets;

// See vertical variant for comments.
public sealed class HorizontalContainer : WidgetBase
{
    protected override Vector2 Size { get; set; }
    private readonly float Gap;
    private readonly List<Widget> Children;
    private readonly bool ShareFreeSpace;
    private float OccupiedSpaceAmount = 0f;
    public HorizontalContainer(
        List<Widget> children,
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

        Size = GetSize();
    }
    public override Vector2 GetSize()
    {
        var totalGapAmount = (Children.Count - 1) * Gap;

        OccupiedSpaceAmount = ShareFreeSpace ? totalGapAmount : 0f;

        Vector2 size;
        size.x = totalGapAmount;
        size.y = 0f;

        foreach (var child in Children)
        {
            var childSize = child.GetSize();

            size.x += childSize.x;
            size.y = Mathf.Max(size.y, childSize.y);

            if (ShareFreeSpace)
            {
                OccupiedSpaceAmount += child.GetFixedSize().x;
            }
        }

        return size;
    }
    protected override void DrawContent(Rect rect)
    {
        var xMax = rect.xMax;
        var size = rect.size;
        size.x = Mathf.Max(size.x - OccupiedSpaceAmount, 0f);

        foreach (var child in Children)
        {
            if (rect.x >= xMax)
            {
                break;
            }

            rect.size = child.GetSize(size);

            if (rect.xMax > 0f)
            {
                child.Draw(rect, size);
            }

            rect.x = rect.xMax + Gap;
        }
    }
}
