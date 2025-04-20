using UnityEngine;

namespace Stats.Widgets.Comps;

public class TextureWidgetComp
    : WidgetComp
{
    private readonly Texture2D Tex;
    public TextureWidgetComp(ref IWidget widget, Texture2D tex)
        : base(ref widget)
    {
        Tex = tex;
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        if (Event.current.type == EventType.Repaint)
        {
            GUI.DrawTexture(rect, Tex);
        }

        Widget.Draw(rect, containerSize);
    }
}
