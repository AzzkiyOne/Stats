using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats;

public class Widget_Texture
    : Widget
{
    private readonly Texture2D Tex;
    public float Scale { get; set; } = 1f;
    public Widget_Texture(Texture2D tex) : base([])
    {
        Tex = tex;
    }
    protected override IEnumerable<Rect> GetLayout(Vector2? contentBoxSize)
    {
        yield return Rect.zero;
    }
    protected override void DrawContentBox(Rect contentBox)
    {
        Widgets.DrawTextureFitted(contentBox, Tex, Scale);
    }
}
