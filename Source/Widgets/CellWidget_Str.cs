using UnityEngine;
using Verse;

namespace Stats;

internal sealed class CellWidget_Str : ICellWidget<string>
{
    public string Value { get; }
    public float MinWidth { get; } = TableWidget.CellMinWidth;
    public CellWidget_Str(string value)
    {
        Value = value;
        MinWidth += Text.CalcSize(value).x;
    }
    public void Draw(Rect targetRect)
    {
        Text.Anchor = TextAnchor.LowerLeft;
        Widgets.Label(targetRect.ContractedBy(TableWidget.CellPadding, 0f), Value);
        Text.Anchor = Constants.DefaultTextAnchor;
    }
    public int CompareTo(ICellWidget? other)
    {
        if (other == null)
        {
            return 1;
        }

        return Value.CompareTo(((ICellWidget<string>)other).Value);
    }
}
