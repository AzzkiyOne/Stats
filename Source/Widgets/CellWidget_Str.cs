using UnityEngine;
using Verse;

namespace Stats;

public sealed class CellWidget_Str : ICellWidget
{
    private readonly string Text;
    private readonly string? Tooltip;
    private readonly ThingIconWidget? Icon;
    public float MinWidth =>
        Icon == null
        ? Verse.Text.CalcSize(Text).x
        : TableWidget_Base.RowHeight
          + TableWidget_Base.IconGap
          + Verse.Text.CalcSize(Text).x;
    public CellWidget_Str(
        string text,
        string? tooltip = null,
        ThingIconWidget? icon = null
    )
    {
        Text = text;
        Tooltip = tooltip;
        Icon = icon;
    }
    public void Draw(Rect targetRect)
    {
        Icon?.Draw(targetRect.CutByX(targetRect.height));
        Widgets.Label(targetRect.ContractedBy(TableWidget_Base.CellPadding, 0f), Text);

        if (Tooltip?.Length > 0)
        {
            TooltipHandler.TipRegion(targetRect, Tooltip);
        }
    }
}
