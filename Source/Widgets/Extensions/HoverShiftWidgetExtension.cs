using UnityEngine;
using Verse;

namespace Stats.Widgets.Extensions;

public sealed class HoverShiftWidgetExtension : WidgetExtension
{
    private readonly float AmountHor;
    private readonly float AmountVer;
    internal HoverShiftWidgetExtension(Widget widget, float amountHor, float amountVer) : base(widget)
    {
        AmountHor = amountHor;
        AmountVer = amountVer;
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        if (Mouse.IsOver(rect))
        {
            rect.x += AmountHor;
            rect.y += AmountVer;
        }

        Widget.Draw(rect, containerSize);
    }
}
