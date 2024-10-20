using UnityEngine;
using Verse;

namespace Stats;

public sealed class CellWidget_Num : ICellWidget<float>
{
    public float Value { get; }
    public float MinWidth { get; } = TableWidget_Base.CellMinWidth;
    private readonly string Text;
    public CellWidget_Num(float value, string text)
    {
        Value = value;
        Text = text;
        MinWidth += Verse.Text.CalcSize(text).x;
    }
    public void Draw(Rect targetRect)
    {
        Verse.Text.Anchor = TextAnchor.LowerRight;
        Widgets.Label(targetRect.ContractedBy(TableWidget_Base.CellPadding, 0f), Text);
        Verse.Text.Anchor = Constants.DefaultTextAnchor;
    }
    public int CompareTo(ICellWidget? other)
    {
        if (other == null)
        {
            return 1;
        }

        return Value.CompareTo(((ICellWidget<float>)other).Value);
    }
    public override string ToString()
    {
        return Value.ToString();
    }
}
