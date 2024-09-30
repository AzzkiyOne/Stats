using System;
using UnityEngine;

namespace Stats.Table;

internal interface ICell : IComparable<ICell?>
{
    string Text { get; }
    void Draw(Rect targetRect, Rect contentRect, TextAnchor textAnchor);
}

internal interface ICell<T> : ICell where T : IComparable<T>
{
    T Value { get; }
}
