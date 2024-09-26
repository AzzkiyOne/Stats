using System;
using UnityEngine;

namespace Stats.Table;

public interface ICell : IComparable<ICell?>
{
    string Text { get; }
    void Draw(Rect targetRect, Rect contentRect, TextAnchor textAnchor);
}

public interface ICell<T> : ICell where T : IComparable<T>
{
    T Value { get; }
}
