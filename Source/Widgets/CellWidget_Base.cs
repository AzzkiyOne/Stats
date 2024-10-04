using UnityEngine;

namespace Stats;

internal abstract class CellWidget_Base<T> : ICellWidget<T>
{
    public T Value { get; }
    public string Text { get; }
    public float MinWidth { get; protected set; }
    public CellWidget_Base(T value, string text)
    {
        Value = value;
        Text = text;
        MinWidth = Verse.Text.CalcSize(Text).x;
    }
    public abstract void Draw(Rect targetRect, Rect contentRect, TextAnchor textAnchor);
    public int CompareTo(ICellWidget? other)
    {
        if (other == null)
        {
            return 1;
        }

        return CompareTo((ICellWidget<T>)other);
    }
    public abstract int CompareTo(ICellWidget<T>? other);
}
