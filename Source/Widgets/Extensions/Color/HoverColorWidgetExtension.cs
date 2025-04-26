using UnityEngine;
using Verse;

namespace Stats.Widgets.Extensions.Color;

public sealed class HoverColorWidgetExtension : WidgetExtension
{
    private readonly UnityEngine.Color Color;
    internal HoverColorWidgetExtension(Widget widget, UnityEngine.Color color) : base(widget)
    {
        Color = color;

        Resize();
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        if (Mouse.IsOver(rect))
        {
            var origGUIColor = GUI.color;
            GUI.color = Color;
            Widget.Draw(rect, containerSize);
            GUI.color = origGUIColor;
        }
        else
        {
            Widget.Draw(rect, containerSize);
        }
    }
}
