using UnityEngine;

namespace Stats.Widgets.Extensions;

public sealed class BorderTopWidgetExtension : WidgetExtension
{
    private readonly float Thickness;
    private readonly Color Color;
    internal BorderTopWidgetExtension(Widget widget, float thickness, Color color) : base(widget)
    {
        Thickness = thickness;
        Color = color;
    }
    protected override Vector2 CalcSize(Vector2 containerSize)
    {
        var size = base.CalcSize(containerSize);
        size.y += Thickness;

        return size;
    }
    protected override Vector2 CalcSize()
    {
        var size = base.CalcSize();
        size.y += Thickness;

        return size;
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        if (Event.current.type == EventType.Repaint)
        {
            Verse.Widgets.DrawBoxSolid(
                rect with { height = Thickness },
                Color.AdjustedForGUIOpacity()
            );
        }

        rect.y += Thickness;
        rect.height -= Thickness;

        Widget.Draw(rect, containerSize);
    }
}
