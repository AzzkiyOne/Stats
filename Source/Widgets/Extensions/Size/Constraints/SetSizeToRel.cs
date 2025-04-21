using UnityEngine;

namespace Stats.Widgets.Extensions.Size.Constraints;

public sealed class SetSizeToRel
    : WidgetExtension
{
    private readonly float WidthMult;
    private readonly float HeightMult;
    internal SetSizeToRel(IWidget widget, float widthMult, float heightMult)
        : base(widget)
    {
        WidthMult = widthMult;
        HeightMult = heightMult;
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        Vector2 size;
        size.x = WidthMult * containerSize.x;
        size.y = HeightMult * containerSize.y;

        return size;
    }
}

public static partial class WidgetAPI
{
    public static SetSizeToRel SizeRel(
        this IWidget widget,
        float widthMult,
        float heightMult
    )
    {
        return new(widget, widthMult, heightMult);
    }
    public static SetSizeToRel SizeRel(this IWidget widget, float sizeMult)
    {
        return widget.SizeRel(sizeMult, sizeMult);
    }
}
