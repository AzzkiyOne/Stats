using System.Collections.Generic;
using UnityEngine;

namespace Stats;

public abstract class Widget_Container
    : Widget
{
    protected List<Widget> Children { get; }
    private Vector2? _ContentSize;
    protected override Vector2 ContentSize
    {
        get
        {
            if (_ContentSize is Vector2 cs) return cs;

            return (Vector2)(_ContentSize = CalcContentSize());
        }
    }
    public Widget_Container(List<Widget> children, WidgetStyle? style = null)
        : base(style)
    {
        Children = children;
    }
    protected abstract Vector2 CalcContentSize();
}
