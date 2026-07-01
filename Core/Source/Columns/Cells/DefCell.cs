using Stats.Utils;
using UnityEngine;

namespace Stats.Columns.Cells;

public interface IDefCell : ICell
{
    public Verse.Def? Value { get; }
    public string? Text { get; }
}

public readonly struct DefCell : IDefCell
{
    public float Width { get; }
    public bool IsRefreshable => false;
    public Verse.Def? Value { get; }
    public string? Text { get; }

    public DefCell(Verse.Def value)
    {
        Value = value;
        Text = value.LabelCap;
        Width = Verse.Text.CalcSize(Text).x;
    }

    public void Draw(Rect rect)
    {
        if (Text != null)
        {
            rect.Label(Text, GUIStyles.TableCell.String);
        }
    }
}
