using UnityEngine;
using Verse;

namespace Stats.Widgets;

// Instead of scale, use padding.
public sealed class Icon : Widget
{
    public Texture2D Texture { get; set; }
    private readonly float Scale;
    public Icon(Texture2D texture, float scale = 1f)
    {
        Texture = texture;
        Scale = scale;
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

        Verse.Widgets.DrawTextureFitted(rect, Texture, Scale);
    }
}
