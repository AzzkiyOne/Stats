using UnityEngine;
using Verse;

namespace Stats.Widgets.Extensions;

public sealed class HoverBackgroundWidgetExtension
    : WidgetExtension
{
    private readonly Texture2D Texture;
    internal HoverBackgroundWidgetExtension(IWidget widget, Texture2D texture)
        : base(widget)
    {
        Texture = texture;
    }
    public override void Draw(Rect rect, Vector2 containerSize)
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
