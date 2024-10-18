using System;
using UnityEngine;

namespace Stats;

public interface ICellWidget : IComparable<ICellWidget?>
{
    float MinWidth { get; }
    void Draw(Rect targetRect);
}

public interface ICellWidget<T> : ICellWidget
{
    T Value { get; }
}
