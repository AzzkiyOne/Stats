using System.Collections.Generic;
using UnityEngine;

namespace Stats.Widgets;

public sealed class VerticalContainer : Widget
{
    private readonly float Gap;
    private readonly List<Widget> Children;
    private readonly float OccupiedSpaceAmount;
    private readonly float TotalGapAmount;
    public VerticalContainer(
        List<Widget> children,
        float gap = 0f,
        bool shareFreeSpace = false
    )
    {
        Children = children;
        Gap = gap;
        TotalGapAmount = (Children.Count - 1) * Gap;

        // Total gap amount is not reserved in normal mode, because overflow may
        // not happen when it should.
        if (shareFreeSpace)
        {
            OccupiedSpaceAmount = TotalGapAmount;
        }

        foreach (var child in children)
        {
            child.Parent = this;

            if (shareFreeSpace)
            {
                OccupiedSpaceAmount += child.GetFixedSize().y;
            }
        }
    }
    protected override Vector2 CalcSize()
    {
        Vector2 size;
        size.x = 0f;
        size.y = TotalGapAmount;

        foreach (var child in Children)
        {
            var childSize = child.GetSize();

            size.x = Mathf.Max(size.x, childSize.x);
            size.y += childSize.y;
        }

        return size;
    }
    public override void Draw(Rect rect, Vector2 _)
    {
        GUIDebugger.DebugRect(this, rect);

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
