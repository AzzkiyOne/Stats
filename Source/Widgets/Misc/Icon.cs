using UnityEngine;
using Verse;

namespace Stats.Widgets.Misc;

// Instead of scale, use padding.
public class Icon
    : Widget
{
    protected override Vector2 Size { get; set; }
    public Texture2D Texture { get; set; }
    public Icon(Texture2D tex)
    {
        Texture = tex;
        Size = GetSize();
    }
    public override Vector2 GetSize()
    {
        return new Vector2(Text.LineHeight, Text.LineHeight);
    }
    protected override void DrawContent(Rect rect)
    {
        if (Event.current.type != EventType.Repaint)
        {
            return;
        }

        GUI.DrawTexture(rect, Texture);
    }
}
