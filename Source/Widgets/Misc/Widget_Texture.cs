using UnityEngine;
using Verse;

namespace Stats;

// This is basically HTML <img>
public class Widget_Texture
    : Widget
{
    private readonly Texture2D Tex;
    private readonly float Scale;
    protected override Vector2 ContentSize => Vector2.zero;
    public Widget_Texture(Texture2D tex, float scale = 1f)
    {
        Tex = tex;
        Scale = scale;
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        base.Draw(rect, containerSize);

        Widgets.DrawTextureFitted(rect, Tex, Scale);
    }
}
