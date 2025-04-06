using UnityEngine;

namespace Stats;

public class WidgetComp_Height_Abs
    : WidgetComp
{
    private readonly float Value;
    public WidgetComp_Height_Abs(IWidget widget, float value)
        : base(widget)
    {
        Value = value;
        widget.HeightIsUndef = false;
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        Vector2 size = Widget.GetSize(containerSize);

        size.y = Value;

        return size;
    }
}
