using UnityEngine;
using Verse;

namespace Stats;

public sealed class Widget_Label_Temp : IWidget
{
    private readonly string Text;
    private readonly string? Tooltip;
    private readonly Widget_ThingIcon? Icon;
    public float MinWidth =>
        Icon == null
            ? Verse.Text.CalcSize(Text).x
            : Widget_Table.RowHeight
              + Widget_Table.IconGap
              + Verse.Text.CalcSize(Text).x;
    public ColumnCellStyle Style { get; init; } = ColumnCellStyle.String;
    public Widget_Label_Temp(
        string text,
        string? tooltip = null,
        Widget_ThingIcon? icon = null
    )
    {
        Text = text;
        Tooltip = tooltip;
        Icon = icon;
    }
    public void Draw(Rect targetRect)
    {
        if (Tooltip?.Length > 0)
        {
            TooltipHandler.TipRegion(targetRect, Tooltip);
        }

        if (Icon != null)
        {
            Icon.Draw(targetRect.CutByX(targetRect.height));
            targetRect.PadLeft(Widget_Table.IconGap);
        }

        Verse.Text.Anchor = (TextAnchor)Style;
        Widgets.Label(targetRect, Text);
        Verse.Text.Anchor = Constants.DefaultTextAnchor;
    }
}
