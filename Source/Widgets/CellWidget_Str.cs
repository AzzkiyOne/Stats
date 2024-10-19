using UnityEngine;
using Verse;

namespace Stats;

public sealed class CellWidget_Str : ICellWidget<string>
{
    public string Value { get; }
    private readonly string? Explanation;
    public float MinWidth { get; } = TableWidget.CellMinWidth;
    public CellWidget_Str(string value, string? explanation = null)
    {
        Value = value;
        Explanation = explanation;
        MinWidth += Text.CalcSize(value).x;
    }
    public void Draw(Rect targetRect)
    {
        Text.Anchor = TextAnchor.LowerLeft;
        Widgets.Label(targetRect.ContractedBy(TableWidget.CellPadding, 0f), Value);
        Text.Anchor = Constants.DefaultTextAnchor;

        if (Explanation != null)
        {
            TooltipHandler.TipRegion(targetRect, Explanation);
        }
    }
    public int CompareTo(ICellWidget? other)
    {
        if (other == null)
        {
            return 1;
        }

        return Value.CompareTo(((ICellWidget<string>)other).Value);
    }
    public override string ToString()
    {
        return Value;
    }
}
