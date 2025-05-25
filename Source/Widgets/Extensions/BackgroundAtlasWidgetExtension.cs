using UnityEngine;

namespace Stats.Widgets.Extensions;

public sealed class BackgroundAtlasWidgetExtension : WidgetExtension
{
    private readonly Texture2D AtlasTexture;
    internal BackgroundAtlasWidgetExtension(Widget widget, Texture2D atlasTexture) : base(widget)
    {
        AtlasTexture = atlasTexture;
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        if (Event.current.type == EventType.Repaint)
        {
            Verse.Widgets.DrawAtlas(rect, AtlasTexture);
        }

        Widget.Draw(rect, containerSize);
    }
}
