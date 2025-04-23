using UnityEngine;

namespace Stats.Widgets.Extensions.Background;

public sealed class BackgroundWidgetExtension : WidgetExtension
{
    private readonly Texture2D Texture;
    internal BackgroundWidgetExtension(Widget widget, Texture2D texture) : base(widget)
    {
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
