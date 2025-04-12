using UnityEngine;
using Verse;

namespace Stats;

// "Icon" is more approriate name, since Verse.Widgets.DrawTextureFitted
// is used to draw the texture.
public class Widget_Icon
    : Widget
{
    protected override Vector2 Size { get; set; }
    public Texture2D Tex { get; set; }
    private readonly float Scale;
    public Widget_Icon(Texture2D tex, float scale = 1f)
    {
        Tex = tex;
        Scale = scale;
        Size = GetSize();
    }
    public override Vector2 GetSize()
    {
        return new Vector2(Text.LineHeight, Text.LineHeight);
    }
    protected override void DrawContent(Rect rect)
    {
        Widgets.DrawTextureFitted(rect, Tex, Scale);
    }
}
