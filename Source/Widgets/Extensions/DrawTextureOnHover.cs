using UnityEngine;
using Verse;

namespace Stats.Widgets.Extensions;

public sealed class DrawTextureOnHover
    : WidgetExtension
{
    private readonly Texture2D Texture;
    internal DrawTextureOnHover(IWidget widget, Texture2D texture)
        : base(widget)
    {
        Texture = texture;
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        if
        (
            Event.current.type == EventType.Repaint
            &&
            Mouse.IsOver(rect)
        )
        {
            GUI.DrawTexture(rect, Texture);
        }

        Widget.Draw(rect, containerSize);
    }
}

public static partial class WidgetAPI
{
    public static DrawTextureOnHover HoverBackground(
        this IWidget widget,
        Texture2D texture
    )
    {
        return new(widget, texture);
    }
}
