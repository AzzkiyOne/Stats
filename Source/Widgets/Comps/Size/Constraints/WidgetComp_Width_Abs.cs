using UnityEngine;

namespace Stats;

public class WidgetComp_Width_Abs
    : WidgetComp
{
    private readonly float Value;
    public WidgetComp_Width_Abs(IWidget widget, float value)
        : base(widget)
    {
        Value = value;
        widget.WidthIsUndef = false;
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        Vector2 size = Widget.GetSize(containerSize);

        size.x = Value;

        return size;
    }
}
