using System;

namespace Stats.Widgets.Table.Filters;

public interface IFilterExpression
{
    bool IsActive { get; }
    event Action<IFilterExpression> OnChange;
    bool Match(ThingAlike thing);
    void Reset();
}
