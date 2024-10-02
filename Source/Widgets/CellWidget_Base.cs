using System;
using UnityEngine;

namespace Stats;

internal abstract class CellWidget_Base<T> : ICellWidget<T> where T : IComparable<T>
{
    public T Value { get; }
    public string Text { get; }
    public CellWidget_Base(T value, string text)
    {
        Value = value;
        Text = text;
    }
    public abstract void Draw(Rect targetRect, Rect contentRect, TextAnchor textAnchor);
    public int CompareTo(ICellWidget? other)
    {
        if (other == null)
        {
            return 1;
        }

        return Value.CompareTo(((ICellWidget<T>)other).Value);
    }
}
