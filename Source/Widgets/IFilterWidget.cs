using UnityEngine;

namespace Stats;

public interface IFilterWidget
{
    bool WasUpdated { get; set; }
    bool HasValue { get; }
    bool Match(ThingRec thing);
    void Draw(Rect targetRect);
}
