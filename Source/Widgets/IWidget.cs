using UnityEngine;

namespace Stats;

public interface IWidget
{
    float MinWidth { get; }
    void Draw(Rect targetRect);
}
