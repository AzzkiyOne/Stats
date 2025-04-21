using UnityEngine;

namespace Stats.Widgets.Comps.Size.Constraints;

public class SetSizeToRel
    : WidgetComp
{
    private readonly float WidthMult;
    private readonly float HeightMult;
    public SetSizeToRel(ref IWidget widget, float widthMult, float heightMult)
        : base(ref widget)
    {
        WidthMult = widthMult;
        HeightMult = heightMult;
    }
    public SetSizeToRel(ref IWidget widget, float sizeMult)
        : this(ref widget, sizeMult, sizeMult)
    {
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        Vector2 size;
        size.x = WidthMult * containerSize.x;
        size.y = HeightMult * containerSize.y;

        return size;
    }
}
