using UnityEngine;
using Verse;

namespace Stats.Widgets;

// Instead of scale, use padding.
public sealed class Icon : Widget
{
    public Texture2D Texture { get; set; }
    public Icon(Texture2D texture)
    {
        Texture = texture;

        Resize();
    }
    public Icon(Texture2D texture, out Icon iconWidget) : this(texture)
    {
        iconWidget = this;
    }
    protected override Vector2 CalcSize()
    {
        return new Vector2(Text.LineHeight, Text.LineHeight);
    }
    public override void Draw(Rect rect, Vector2 _)
    {
        GUIDebugger.DebugRect(this, rect);

        if (Event.current.type != EventType.Repaint)
        {
            return;
        }

        GUI.DrawTexture(rect, Texture);
    }
}
