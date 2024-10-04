using System;
using UnityEngine;

namespace Stats;

internal interface ICellWidget : IComparable<ICellWidget?>
{
    string Text { get; }
    float MinWidth { get; }
    void Draw(Rect targetRect, Rect contentRect, TextAnchor textAnchor);
}

internal interface ICellWidget<T> : ICellWidget, IComparable<ICellWidget<T>?>
{
    T Value { get; }
}
