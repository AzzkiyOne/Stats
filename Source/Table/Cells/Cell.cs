using System;
using UnityEngine;
using Verse;

namespace Stats.Table.Cells;

public sealed class Cell<T> : ICell<T> where T : notnull, IComparable<T>
{
    public T Value { get; }
    private string Text { get; }
    private string Tip { get; }
    public Cell(T value, string text, string tip = "")
    {
        Value = value;
        Text = text;
        Tip = tip;
    }
    public Cell(T value, string tip = "")
    {
        Value = value;
        Text = Value.ToString();
        Tip = tip;
    }
    public void Draw(Rect targetRect, Rect contentRect, TextAnchor textAnchor)
    {
        using (new TextAnchorCtx(textAnchor))
        {
            Widgets.Label(contentRect, Text);
        }

        TooltipHandler.TipRegion(targetRect, new TipSignal(Tip));
    }
    public int CompareTo(ICell? other)
    {
        if (other == null)
        {
            return 1;
        }

        return Value.CompareTo(((ICell<T>)other).Value);
    }
}
