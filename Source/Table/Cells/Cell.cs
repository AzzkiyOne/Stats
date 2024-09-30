using System;
using UnityEngine;

namespace Stats.Table.Cells;

internal abstract class Cell<T> : ICell<T> where T : IComparable<T>
{
    public T Value { get; }
    public string Text { get; }
    public Cell(T value, string text)
    {
        Value = value;
        Text = text;
    }
    public abstract void Draw(Rect targetRect, Rect contentRect, TextAnchor textAnchor);
    public int CompareTo(ICell? other)
    {
        if (other == null)
        {
            return 1;
        }

        return Value.CompareTo(((ICell<T>)other).Value);
    }
}
