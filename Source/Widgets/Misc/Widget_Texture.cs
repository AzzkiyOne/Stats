using UnityEngine;
using Verse;

namespace Stats;

// This is basically HTML <img>
public class Widget_Texture
    : Widget
{
    public Texture2D Tex { get; set; }
    private readonly float Scale;
    public Widget_Texture(Texture2D tex, float scale = 1f)
    {
        Tex = tex;
        Scale = scale;
    }
    protected override Vector2 GetSize()
    {
        return Vector2.zero;
    }
    protected override void DrawContent(Rect rect)
    {
        Widgets.DrawTextureFitted(rect, Tex, Scale);
    }
}
