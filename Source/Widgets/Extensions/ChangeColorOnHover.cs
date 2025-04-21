using UnityEngine;
using Verse;

namespace Stats.Widgets.Extensions;

public sealed class ChangeColorOnHover
    : WidgetExtension
{
    private readonly Color Color;
    internal ChangeColorOnHover(IWidget widget, Color color)
        : base(widget)
    {
        Color = color;
    }
    public ChangeColorOnHover(ref IWidget widget)
        : this(widget, GenUI.MouseoverColor)
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

public static partial class WidgetAPI
{
    public static ChangeColorOnHover HoverColor(this IWidget widget, Color color)
    {
        return new(widget, color);
    }
}
