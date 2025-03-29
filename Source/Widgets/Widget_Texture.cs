using UnityEngine;
using Verse;

namespace Stats;

// This is basically HTML <img>
public class Widget_Texture
    : Widget_Drawable
{
    private readonly Texture2D Tex;
    public float Scale { get; set; } = 1f;
    protected override Vector2 ContentSize => Vector2.zero;
    public Widget_Texture(Texture2D tex, WidgetStyle? style = null)
        : base(style)
    {
        Tex = tex;
    }
    public override void Draw(Rect rect)
    {
        base.Draw(rect);

        Widgets.DrawTextureFitted(rect, Tex, Scale);
    }
}
