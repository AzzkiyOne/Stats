using UnityEngine;

namespace Stats.Widgets.Extensions.Background;

public sealed class BackgroundWidgetExtension : WidgetExtension
{
    private readonly Texture2D Texture;
    private readonly UnityEngine.Color Color;
    internal BackgroundWidgetExtension(Widget widget, Texture2D texture, UnityEngine.Color color) : base(widget)
    {
        Texture = texture;
        Color = color;

        Resize();
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        if (Event.current.type == EventType.Repaint)
        {
            GUI.DrawTexture(
                rect, Texture, ScaleMode.StretchToFill, true, 0f, Color.AdjustedForGUIOpacity(), 0f, 0f
            );
        }

        Widget.Draw(rect, containerSize);
    }
}
