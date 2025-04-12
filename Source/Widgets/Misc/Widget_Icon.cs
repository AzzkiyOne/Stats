using UnityEngine;
using Verse;

namespace Stats;

// Instead of scale, use padding.
public class Widget_Icon
    : Widget
{
    protected override Vector2 Size { get; set; }
    public Texture2D Tex { get; set; }
    public Widget_Icon(Texture2D tex)
    {
        Tex = tex;
        Size = GetSize();
    }
    public override Vector2 GetSize()
    {
        return new Vector2(Text.LineHeight, Text.LineHeight);
    }
    protected override void DrawContent(Rect rect)
    {
        GUI.DrawTexture(rect, Tex);
    }
}
