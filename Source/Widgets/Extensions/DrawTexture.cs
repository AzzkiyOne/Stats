using UnityEngine;

namespace Stats.Widgets.Extensions;

public sealed class DrawTexture
    : WidgetExtension
{
    private readonly Texture2D Texture;
    internal DrawTexture(IWidget widget, Texture2D texture)
        : base(widget)
    {
        Texture = texture;
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        if (Event.current.type == EventType.Repaint)
        {
            GUI.DrawTexture(rect, Texture);
        }

        Widget.Draw(rect, containerSize);
    }
}

public static partial class WidgetAPI
{
    public static DrawTexture Background(this IWidget widget, Texture2D texture)
    {
        return new(widget, texture);
    }
}
