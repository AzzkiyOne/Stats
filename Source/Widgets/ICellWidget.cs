using System;
using UnityEngine;

namespace Stats;

internal interface ICellWidget : IComparable<ICellWidget?>
{
    string Text { get; }
    void Draw(Rect targetRect, Rect contentRect, TextAnchor textAnchor);
}

internal interface ICellWidget<T> : ICellWidget where T : IComparable<T>
{
    T Value { get; }
}
