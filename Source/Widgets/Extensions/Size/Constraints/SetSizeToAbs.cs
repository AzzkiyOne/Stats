using UnityEngine;

namespace Stats.Widgets.Extensions.Size.Constraints;

public sealed class SetSizeToAbs
    : WidgetExtension
{
    private readonly Vector2 Size;
    internal SetSizeToAbs(IWidget widget, float width, float height)
        : base(widget)
    {
        Size.x = width;
        Size.y = height;
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

public static partial class WidgetAPI
{
    public static SetSizeToAbs SizeAbs(
        this IWidget widget,
        float widthMult,
        float heightMult
    )
    {
        return new(widget, widthMult, heightMult);
    }
    public static SetSizeToAbs SizeAbs(this IWidget widget, float sizeMult)
    {
        return widget.SizeAbs(sizeMult, sizeMult);
    }
}
