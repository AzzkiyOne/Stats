using Stats.Utils;
using Stats.Utils.Extensions;
using UnityEngine;

namespace Stats.Columns.Cells;

public interface INumberCell : ICell
{
    public decimal Value { get; }
}

public readonly struct NumberCell : INumberCell
{
    public float Width { get; }
    public bool IsRefreshable => false;
    public decimal Value { get; }

    private readonly string? _text;

    public NumberCell(float value, string formatString = "") : this(value.ToDecimal(0), formatString) { }

    public NumberCell(decimal value, string formatString = "")
    {
        Value = value;
        if (value != 0m)
        {
            _text = value.ToString(formatString);
            Width = _text.CalcSize(GUIStyles.TableCell.NumberNoPad).x;
        }
    }

    public void Draw(Rect rect)
    {
        if (_text != null)
        {
            rect.Label(_text, GUIStyles.TableCell.Number);
        }
    }
}
