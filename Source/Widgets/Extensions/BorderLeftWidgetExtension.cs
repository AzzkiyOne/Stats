using UnityEngine;

namespace Stats.Widgets.Extensions;

public sealed class BorderLeftWidgetExtension : WidgetExtension
{
    private readonly float Thickness;
    private readonly Color Color;
    internal BorderLeftWidgetExtension(Widget widget, float thickness, Color color) : base(widget)
    {
        Thickness = thickness;
        Color = color;
    }
    protected override Vector2 CalcSize(Vector2 containerSize)
    {
        var size = base.CalcSize(containerSize);
        size.x += Thickness;

        return size;
    }
    protected override Vector2 CalcSize()
    {
        var size = base.CalcSize();
        size.x += Thickness;

        return size;
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        if (Event.current.type == EventType.Repaint)
        {
            Verse.Widgets.DrawBoxSolid(
                rect with { width = Thickness },
                Color.AdjustedForGUIOpacity()
            );
        }

        rect.x += Thickness;
        rect.width -= Thickness;

        Widget.Draw(rect, containerSize);
    }
}
