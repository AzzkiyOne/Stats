using UnityEngine;
using Verse;

namespace Stats.Widgets.Comps;

public class TextureAlternatingWidgetComp
    : WidgetComp
{
    private readonly Texture2D TexIdle;
    private readonly Texture2D TexHover;
    public TextureAlternatingWidgetComp(
        ref IWidget widget,
        Texture2D texIdle,
        Texture2D texHover
    )
        : base(ref widget)
    {
        TexIdle = texIdle;
        TexHover = texHover;
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        if (Event.current.type == EventType.Repaint)
        {
            if (Mouse.IsOver(rect))
            {
                GUI.DrawTexture(rect, TexHover);
            }
            else
            {
                GUI.DrawTexture(rect, TexIdle);
            }
        }

        Widget.Draw(rect, containerSize);
    }
}
