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
    }
    public WidgetComp_Size_Abs(IWidget widget, float size)
        : base(widget)
    {
        Width = Height = size;
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        Vector2 size;

        size.x = Width;
        size.y = Height;

        return size;
    }
    public override Vector2 GetSize()
    {
        Vector2 size;

        size.x = Width;
        size.y = Height;

        return size;
    }
}
