using UnityEngine;
using Verse;

namespace Stats;

// This is basically HTML <img>
public class Widget_Texture
    : Widget
{
    private readonly Texture2D Tex;
    private readonly float Scale;
    public Widget_Texture(Texture2D tex, float scale = 1f)
    {
        Tex = tex;
        Scale = scale;
    }
    protected override void DrawContent(Rect rect)
    {
        Widgets.DrawTextureFitted(rect, Tex, Scale);
    }
}
