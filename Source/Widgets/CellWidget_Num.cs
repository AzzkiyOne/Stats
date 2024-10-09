using UnityEngine;
using Verse;

namespace Stats;

internal sealed class CellWidget_Num : ICellWidget<float>
{
    public float Value { get; }
    public float MinWidth { get; } = TableWidget.CellMinWidth;
    private readonly string Text;
    public Color Color { get; set; } = Color.white;
    public CellWidget_Num(float value, string text)
    {
        Value = value;
        Text = text;
        MinWidth += Verse.Text.CalcSize(text).x;
    }
    public void Draw(Rect targetRect)
    {
        GUI.color = Color;
        Verse.Text.Anchor = TextAnchor.LowerRight;
        Widgets.Label(targetRect.ContractedBy(TableWidget.CellPadding, 0f), Text);
        GUI.color = Color.white;
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
    public void Reset()
    {
        Color = Color.white;
    }
}
