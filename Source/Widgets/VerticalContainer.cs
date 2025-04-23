using System.Collections.Generic;
using UnityEngine;

namespace Stats.Widgets;

public sealed class VerticalContainer : WidgetBase
{
    protected override Vector2 Size { get; set; }
    private readonly float Gap;
    private readonly List<Widget> Children;
    private readonly bool ShareFreeSpace;
    private float OccupiedSpaceAmount = 0f;
    public VerticalContainer(
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
        // Total gap amount is not reserved in normal mode, because overflow may
        // not happen when it should.
        OccupiedSpaceAmount = ShareFreeSpace ? totalGapAmount : 0f;

        Vector2 size;
        size.x = 0f;
        size.y = totalGapAmount;

        foreach (var child in Children)
        {
            var childSize = child.GetSize();

            size.x = Mathf.Max(size.x, childSize.x);
            size.y += childSize.y;

            if (ShareFreeSpace)
            {
                OccupiedSpaceAmount += child.GetFixedSize().y;
            }
        }

        return size;
    }
    protected override void DrawContent(Rect rect)
    {
        var yMax = rect.yMax;
        var size = rect.size;
        size.y = Mathf.Max(size.y - OccupiedSpaceAmount, 0f);

        foreach (var child in Children)
        {
            if (rect.y >= yMax)
            {
                break;
            }

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
