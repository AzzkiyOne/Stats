using UnityEngine;

namespace Stats;

public interface ICellWidget
{
    float MinWidth { get; }
    void Draw(Rect targetRect);
}
