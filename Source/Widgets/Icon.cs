using UnityEngine;
using Verse;

namespace Stats.Widgets;

// Instead of scale, use padding.
public sealed class Icon
    : Widget
{
    protected override Vector2 Size { get; set; }
    public Texture2D Texture { get; set; }
    public Icon(Texture2D texture)
    {
        Texture = texture;
        Size = GetSize();
    }
    public Icon(Texture2D texture, out Icon iconWidget)
        : this(texture)
    {
        iconWidget = this;
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
