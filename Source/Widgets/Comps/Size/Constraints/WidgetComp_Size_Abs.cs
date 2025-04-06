using UnityEngine;

namespace Stats;

public class WidgetComp_Size_Abs
    : WidgetComp
{
    private readonly float Width;
    private readonly float Height;
    public WidgetComp_Size_Abs(IWidget widget, float width, float height)
        : base(widget)
    {
        Width = width;
        Height = height;
        widget.WidthIsUndef = false;
        widget.HeightIsUndef = false;
    }
    public WidgetComp_Size_Abs(IWidget widget, float size)
        : this(widget, size, size)
    {
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        Vector2 size;

        size.x = Width;
        size.y = Height;

        return size;
    }
}
