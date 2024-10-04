using UnityEngine;

namespace Stats;

internal sealed class CellWidget_Diff : ICellWidget<float>
{
    private ICellWidget<float> OrigCell { get; }
    private ICellWidget<float> CurCell { get; set; }
    public float Value => OrigCell.Value;
    public string Text => CurCell.Text;
    public float MinWidth => OrigCell.MinWidth;
    public CellWidget_Diff(ICellWidget<float> cell)
    {
        OrigCell = CurCell = cell;
    }
    public void Switch(CellWidget_Diff other, bool reverseDiffModeColors)
    {
        var value = OrigCell.Value / other.Value;

        if (float.IsFinite(value) == false)
        {
            Reset();

            return;
        }

        var text = $"x {value:0.00}";
        var color = (OrigCell.CompareTo(other) * (reverseDiffModeColors ? -1 : 1)) switch
        {
            < 0 => Color.red,
            > 0 => Color.green,
            0 => Color.yellow,
        };
        var tip = $"This: {OrigCell.Text}\n\nSelected: {other.OrigCell.Text}";

        CurCell = new CellWidget_Gen<float>(value, text, tip, color: color);
    }
    public void Reset()
    {
        CurCell = OrigCell;
    }
    public void Draw(Rect targetRect, Rect contentRect, TextAnchor textAnchor)
    {
        CurCell.Draw(targetRect, contentRect, textAnchor);
    }
    public int CompareTo(ICellWidget? other)
    {
        if (other == null)
        {
            return 1;
        }

        return CompareTo((ICellWidget<float>)other);
    }
    public int CompareTo(ICellWidget<float>? other)
    {
        return OrigCell.CompareTo(other);
    }
}
