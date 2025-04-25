using UnityEngine;
using Verse;

namespace Stats.Widgets.Extensions.Background;

public sealed class HoverBackgroundWidgetExtension : WidgetExtension
{
    private readonly Texture2D Texture;
    private readonly UnityEngine.Color Color;
    internal HoverBackgroundWidgetExtension(Widget widget, Texture2D texture, UnityEngine.Color color) : base(widget)
    {
        Texture = texture;
        Color = color;
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        if (Event.current.type == EventType.Repaint && Mouse.IsOver(rect))
        {
            GUI.DrawTexture(
                rect, Texture, ScaleMode.StretchToFill, true, 0f, Color.AdjustedForGUIOpacity(), 0f, 0f
            );
        }

        Widget.Draw(rect, containerSize);
    }
}
