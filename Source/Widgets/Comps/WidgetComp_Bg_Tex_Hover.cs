using UnityEngine;
using Verse;

namespace Stats.Widgets.Comps;

public class WidgetComp_Bg_Tex_Hover
    : WidgetComp
{
    private readonly Texture2D Tex;
    public WidgetComp_Bg_Tex_Hover(ref IWidget widget, Texture2D tex)
        : base(ref widget)
    {
        Tex = tex;
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        if
        (
            Event.current.type == EventType.Repaint
            &&
            Mouse.IsOver(rect)
        )
        {
            GUI.DrawTexture(rect, Tex);
        }

        Widget.Draw(rect, containerSize);
    }
}
