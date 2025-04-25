using UnityEngine;

namespace Stats.Widgets.Extensions.Border;

public sealed class BorderBottomWidgetExtension : WidgetExtension
{
    private readonly float Thickness;
    private readonly UnityEngine.Color Color;
    internal BorderBottomWidgetExtension(Widget widget, float thickness, UnityEngine.Color color) : base(widget)
    {
        Thickness = thickness;
        Color = color;
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        if (Event.current.type == EventType.Repaint)
        {
            Verse.Widgets.DrawBoxSolid(
                rect with { y = rect.yMax - Thickness, height = Thickness },
                Color.AdjustedForGUIOpacity()
            );
        }

        Widget.Draw(rect, containerSize);
    }
}
