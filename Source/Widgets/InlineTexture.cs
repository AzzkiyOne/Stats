using UnityEngine;
using Verse;

namespace Stats.Widgets;

// Designed for rendering textures along with text.
public class InlineTexture : Widget
{
    public Texture2D Texture { get; set; }
    private readonly float Scale;
    public InlineTexture(Texture2D texture, float scale = 1f)
    {
        Texture = texture;
        Scale = scale;
    }
    public InlineTexture(Texture2D texture, out InlineTexture instance) : this(texture)
    {
        instance = this;
    }
    protected override Vector2 CalcSize()
    {
        var width = Text.LineHeight * (Texture.width / (float)Texture.height);

        return new Vector2(width, Text.LineHeight);
    }
    public override void Draw(Rect rect, Vector2 _)
    {
        GUIDebugger.DebugRect(this, rect);

        if (Event.current.type != EventType.Repaint)
        {
            return;
        }

        // TODO: See if this is the most appropriate method.
        Verse.Widgets.DrawTextureFitted(rect, Texture, Scale);
    }
}
