using UnityEngine;
using Verse;

namespace Stats.Widgets.Comps;

public class SetColorOnHover
    : WidgetComp
{
    private readonly Color Color;
    public SetColorOnHover(ref IWidget widget, Color color)
        : base(ref widget)
    {
        Color = color;
    }
    public SetColorOnHover(ref IWidget widget)
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
