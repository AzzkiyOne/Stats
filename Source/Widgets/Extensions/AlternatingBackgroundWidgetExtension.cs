using UnityEngine;
using Verse;

namespace Stats.Widgets.Extensions;

public sealed class AlternatingBackgroundWidgetExtension
    : WidgetDecorator
{
    public override Widget Widget { get; }
    private readonly Texture2D IdleTexture;
    private readonly Texture2D HoverTexture;
    internal AlternatingBackgroundWidgetExtension(
        Widget widget,
        Texture2D idleTexture,
        Texture2D hoverTexture
    )
    {
        Widget = widget;
        IdleTexture = idleTexture;
        HoverTexture = hoverTexture;
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        if (Event.current.type == EventType.Repaint)
        {
            if (Mouse.IsOver(rect))
            {
                GUI.DrawTexture(rect, HoverTexture);
            }
            else
            {
                GUI.DrawTexture(rect, IdleTexture);
            }
        }

        Widget.Draw(rect, containerSize);
    }
}
