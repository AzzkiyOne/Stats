using UnityEngine;
using Verse;

namespace Stats.Widgets.Comps;

public class DrawTextureOnHover
    : WidgetComp
{
    private readonly Texture2D Texture;
    public DrawTextureOnHover(ref IWidget widget, Texture2D texture)
        : base(ref widget)
    {
        Texture = texture;
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
            GUI.DrawTexture(rect, Texture);
        }

        Widget.Draw(rect, containerSize);
    }
}
