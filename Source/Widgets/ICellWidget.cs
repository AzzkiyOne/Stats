using System;
using UnityEngine;

namespace Stats;

internal interface ICellWidget : IComparable<ICellWidget?>
{
    float MinWidth { get; }
    void Draw(Rect targetRect, Rect contentRect, TextAnchor textAnchor);
}

internal interface ICellWidget<T> : ICellWidget
{
    T Value { get; }
}
