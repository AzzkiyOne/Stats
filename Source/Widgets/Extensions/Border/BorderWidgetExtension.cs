using UnityEngine;

namespace Stats.Widgets.Extensions.Border;

public sealed class BorderWidgetExtension : WidgetExtension
{
    private readonly float Thickness;
    private readonly UnityEngine.Color Color;
    internal BorderWidgetExtension(Widget widget, float thickness, UnityEngine.Color color) : base(widget)
    {
        Thickness = thickness;
        Color = color;
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        if (Event.current.type == EventType.Repaint)
        {
            var color = Color.AdjustedForGUIOpacity();
            // Hor:
            // - Top
            var horRect = rect with { height = Thickness };
            Verse.Widgets.DrawBoxSolid(horRect, color);
            // - Bottom
            horRect.y = rect.yMax - Thickness;
            Verse.Widgets.DrawBoxSolid(horRect, color);
            // Ver:
            // - Left
            var verRect = rect with { width = Thickness };
            Verse.Widgets.DrawBoxSolid(verRect, color);
            // - Right
            verRect.x = rect.xMax - Thickness;
            Verse.Widgets.DrawBoxSolid(verRect, color);
        }

        Widget.Draw(rect, containerSize);
    }
}
