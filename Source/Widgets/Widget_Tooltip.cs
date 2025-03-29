using UnityEngine;
using Verse;

namespace Stats;

public class Widget_Tooltip
    : Widget
{
    private readonly Widget Widget;
    private readonly string Text;
    public Widget_Tooltip(Widget widget, string text)
        : base(widget.Style)
    {
        Widget = widget;
        Text = text;
    }
    public override Vector2 ContentSize => Widget.ContentSize;
    public override void DrawBorderBox(Rect borderBox)
    {
        base.DrawBorderBox(borderBox);

        TooltipHandler.TipRegion(borderBox, Text);
    }
    public override void DrawContentBox(Rect contentBox)
    {
        Widget.DrawContentBox(contentBox);
    }
}
