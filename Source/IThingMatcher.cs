using System;

namespace Stats;

public interface IThingMatcher
{
    event Action OnChange;
    bool Match(ThingRec thing);
    void Reset();
}
