﻿using System.Collections.Generic;
using UnityEngine;

namespace Stats.Widgets;

public sealed class VerticalContainer : Widget
{
    private readonly List<Widget> Children;
    private readonly float Gap;
    private readonly float TotalGapAmount;
    private readonly bool ShareFreeSpace;
    private readonly bool StretchItems;
    private float OccupiedSpaceAmount;
    public VerticalContainer(
        List<Widget> children,
        float gap = 0f,
        bool shareFreeSpace = false,
        bool stretchItems = false
    )
    {
        Children = children;
        Gap = gap;
        TotalGapAmount = (Children.Count - 1) * Gap;
        ShareFreeSpace = shareFreeSpace;
        StretchItems = stretchItems;

        foreach (var child in children)
        {
            child.Parent = this;
        }
    }
    protected override Vector2 CalcSize()
    {
        Vector2 size;
        size.x = 0f;
        size.y = TotalGapAmount;

        // Total gap amount is not reserved in normal mode, because overflow may
        // not happen when it should.
        if (ShareFreeSpace || StretchItems)
        {
            OccupiedSpaceAmount = TotalGapAmount;
        }

        foreach (var child in Children)
        {
            var childSize = child.GetSize();

            size.x = Mathf.Max(size.x, childSize.x);
            size.y += childSize.y;

            if (ShareFreeSpace || StretchItems)
            {
                OccupiedSpaceAmount += child.GetFixedSize().y;
            }
        }

        return size;
    }
    public override void Draw(Rect rect, Vector2 _)
    {
        GUIDebugger.DebugRect(this, rect);

        var yMax = rect.yMax;
        var size = rect.size;
        size.y = Mathf.Max(size.y - OccupiedSpaceAmount, 0f);
        var additionalChildHeight = 0f;

        if (StretchItems)
        {
            additionalChildHeight = Mathf.Floor(size.y / Children.Count);
        }

        foreach (var child in Children)
        {
            if (rect.y >= yMax)
            {
                break;
            }

            rect.size = child.GetSize(size);
            rect.height += additionalChildHeight;

            // This is for (future) scroll component.
            if (rect.yMax > 0f)
            {
                child.Draw(rect, size);
            }

            rect.y = rect.yMax + Gap;
        }
    }
}
