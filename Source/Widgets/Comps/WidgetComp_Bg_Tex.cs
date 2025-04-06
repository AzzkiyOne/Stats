using UnityEngine;

namespace Stats;

public class WidgetComp_Bg_Tex
    : WidgetComp
{
    private readonly Texture2D Tex;
    public WidgetComp_Bg_Tex(IWidget widget, Texture2D tex)
        : base(widget)
    {
        Tex = tex;
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        GUI.DrawTexture(rect, Tex);

        Widget.Draw(rect, containerSize);
    }
}
