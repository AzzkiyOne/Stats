using UnityEngine;
using Verse;

namespace Stats.Widgets.Extensions;

public sealed class HoverShiftHorWidgetExtension : WidgetExtension
{
    private readonly float Amount;
    internal HoverShiftHorWidgetExtension(Widget widget, float amount) : base(widget)
    {
        Amount = amount;

        Resize();
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        if (Mouse.IsOver(rect))
        {
            rect.x += Amount;
        }

        Widget.Draw(rect, containerSize);
    }
}
