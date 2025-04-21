using UnityEngine;

namespace Stats.Widgets.Comps;

public class DrawTexture
    : WidgetComp
{
    private readonly Texture2D Texture;
    public DrawTexture(ref IWidget widget, Texture2D texture)
        : base(ref widget)
    {
        Texture = texture;
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        if (Event.current.type == EventType.Repaint)
        {
            GUI.DrawTexture(rect, Texture);
        }

        Widget.Draw(rect, containerSize);
    }
}
