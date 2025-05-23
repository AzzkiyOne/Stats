﻿using UnityEngine;

namespace Stats.Widgets.Extensions;

public sealed class RelSizeWidgetExtension : WidgetExtension
{
    private readonly float ParentWidthMultiplier;
    private readonly float ParentHeightMultiplier;
    internal RelSizeWidgetExtension(
        Widget widget,
        float parentWidthMultiplier,
        float parentHeightMultiplier
    ) : base(widget)
    {
        ParentWidthMultiplier = parentWidthMultiplier;
        ParentHeightMultiplier = parentHeightMultiplier;
    }
    protected override Vector2 CalcSize(Vector2 containerSize)
    {
        Vector2 size;
        size.x = ParentWidthMultiplier * containerSize.x;
        size.y = ParentHeightMultiplier * containerSize.y;

        return size;
    }
}
