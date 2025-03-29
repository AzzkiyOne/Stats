using System.Collections.Generic;
using UnityEngine;

namespace Stats;

public abstract class Widget_Container_XY
    : Widget_Container
{
    protected float Gap { get; }
    protected float TotalGapAmount { get; }
    protected float ReservedSpaceAmount { get; init; }
    public Widget_Container_XY(
        List<Widget> children,
        float gap = 0f,
        WidgetStyle? style = null
    )
        : base(children, style)
    {
        Gap = gap;
        TotalGapAmount = (Children.Count - 1) * gap;
        ReservedSpaceAmount = TotalGapAmount;
    }
}
