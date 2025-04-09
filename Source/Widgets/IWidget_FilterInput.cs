using UnityEngine;

namespace Stats;

public interface IWidget_FilterInput
{
    bool WasUpdated { get; set; }
    bool HasValue { get; }
    bool Match(ThingRec thing);
    void Draw(Rect targetRect);
}
