using UnityEngine;
using Verse;

namespace Stats;

public class WidgetComp_Bg_Tex_Alt
    : WidgetComp
{
    private readonly Texture2D TexIdle;
    private readonly Texture2D TexHover;
    public WidgetComp_Bg_Tex_Alt(
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
        if (Mouse.IsOver(rect))
        {
            GUI.DrawTexture(rect, TexHover);
        }
        else
        {
            GUI.DrawTexture(rect, TexIdle);
        }

        Widget.Draw(rect, containerSize);
    }
}
