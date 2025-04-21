using UnityEngine;
using Verse;

namespace Stats.Widgets.Extensions;

public sealed class ChangeTextureOnHover
    : WidgetExtension
{
    private readonly Texture2D IdleTexture;
    private readonly Texture2D HoverTexture;
    internal ChangeTextureOnHover(
        IWidget widget,
        Texture2D idleTexture,
        Texture2D hoverTexture
    )
        : base(widget)
    {
        IdleTexture = idleTexture;
        HoverTexture = hoverTexture;
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
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

public static partial class WidgetAPI
{
    public static ChangeTextureOnHover Background(
        this IWidget widget,
        Texture2D idleTexture,
        Texture2D hoverTexture
    )
    {
        return new(widget, idleTexture, hoverTexture);
    }
}
