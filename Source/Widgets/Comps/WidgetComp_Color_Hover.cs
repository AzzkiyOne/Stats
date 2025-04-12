using UnityEngine;
using Verse;

namespace Stats;

public class WidgetComp_Color_Hover
    : WidgetComp
{
    private readonly Color Color;
    public WidgetComp_Color_Hover(ref IWidget widget, Color color)
        : base(ref widget)
    {
        Color = color;
    }
    public WidgetComp_Color_Hover(ref IWidget widget)
        : this(ref widget, GenUI.MouseoverColor)
    {
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
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
