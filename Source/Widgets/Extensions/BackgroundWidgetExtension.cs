using UnityEngine;

namespace Stats.Widgets.Extensions;

public sealed class BackgroundWidgetExtension
    : WidgetDecorator
{
    public override Widget Widget { get; }
    private readonly Texture2D Texture;
    internal BackgroundWidgetExtension(Widget widget, Texture2D texture)
    {
        Widget = widget;
        Texture = texture;
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        if (Event.current.type == EventType.Repaint)
        {
            GUI.DrawTexture(rect, Texture);
        }

        Widget.Draw(rect, containerSize);
    }
}
