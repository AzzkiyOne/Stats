using UnityEngine;

namespace Stats.Widgets.Comps.Size.Constraints;

public class SetSizeToAbs
    : WidgetComp
{
    private readonly Vector2 Size;
    public SetSizeToAbs(ref IWidget widget, float width, float height)
        : base(ref widget)
    {
        Size.x = width;
        Size.y = height;
    }
    public SetSizeToAbs(ref IWidget widget, float size)
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
