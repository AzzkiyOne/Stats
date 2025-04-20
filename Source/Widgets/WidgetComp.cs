using UnityEngine;

namespace Stats.Widgets;

public abstract class WidgetComp
    : WidgetDecorator
{
    protected override IWidget Widget { get; }
    public WidgetComp(ref IWidget widget)
    {
        Widget = widget;
        widget = this;
    }
}

public delegate float SizeFunc_SingleAxis(in Vector2 containerSize);
public delegate Vector2 SizeFunc_DoubleAxis(in Vector2 containerSize);
