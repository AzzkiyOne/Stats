using UnityEngine;
using Verse;

namespace Stats;

public class Widget_Texture
    : Widget
{
    private readonly Texture2D Tex;
    public float Scale { get; set; } = 1f;
    protected override Vector2 ContentSize => Vector2.zero;
    public Widget_Texture(Texture2D tex, WidgetStyle? style = null)
        : base(style)
    {
        Tex = tex;
    }
    protected override void DrawContentBox(Rect contentBox)
    {
        Widgets.DrawTextureFitted(contentBox, Tex, Scale);
    }
}
