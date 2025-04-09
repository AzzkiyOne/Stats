using UnityEngine;

namespace Stats;

public class WidgetComp_Size_Abs
    : WidgetComp
{
    private readonly Vector2 Size;
    public WidgetComp_Size_Abs(ref IWidget widget, float width, float height)
        : base(ref widget)
    {
        Size.x = width;
        Size.y = height;
    }
    public WidgetComp_Size_Abs(ref IWidget widget, float size)
        : this(ref widget, size, size)
    {
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        return Size;
    }
    public override Vector2 GetSize()
    {
        return Size;
    }
}
