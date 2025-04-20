using System;

namespace Stats.Widgets.Table.Filters;

public interface IThingMatcher
{
    bool IsActive { get; }
    event Action<IThingMatcher> OnChange;
    bool Match(ThingRec thing);
    void Reset();
}
