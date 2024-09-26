using UnityEngine;

namespace Stats.Table.Cells;

internal sealed class Cell_Diff : ICell<float>
{
    private ICell<float> OrigCell { get; }
    private ICell<float> CurCell { get; set; }
    public float Value => OrigCell.Value;
    public string Text => CurCell.Text;
    public Cell_Diff(ICell<float> cell)
    {
        OrigCell = CurCell = cell;
    }
    public void Switch(Cell_Diff other, bool reverseDiffModeColors)
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

        CurCell = new Cell_Gen<float>(value, text, tip, color: color);
    }
    public void Reset()
    {
        CurCell = OrigCell;
    }
    public void Draw(Rect targetRect, Rect contentRect, TextAnchor textAnchor)
    {
        CurCell.Draw(targetRect, contentRect, textAnchor);
    }
    public int CompareTo(ICell? other)
    {
        return OrigCell.CompareTo(other);
    }
}
