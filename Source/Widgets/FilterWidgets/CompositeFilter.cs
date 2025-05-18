using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Stats.Widgets;

internal sealed class CompositeFilter<TObject> : FilterWidget<TObject>
{
    public override bool IsActive => Filters.Any(filter => filter.IsActive);
    public override event Action<FilterWidget<TObject>>? OnChange;
    private readonly List<FilterWidget<TObject>> Filters;
    private readonly Widget Container;
    public CompositeFilter(List<Widget> filters)
    {
        Filters = filters.Select(widget => widget.Get<FilterWidget<TObject>>()).ToList();

        foreach (var filter in Filters)
        {
            filter.OnChange += filter => OnChange?.Invoke(this);
        }

        Container = new HorizontalContainer(filters, shareFreeSpace: true);
        Container.Parent = this;
    }
    protected override Vector2 CalcSize(Vector2 containerSize)
    {
        return Container.GetSize(containerSize);
    }
    protected override Vector2 CalcSize()
    {
        return Container.GetSize();
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        Container.Draw(rect, containerSize);
    }
    public override bool Eval(TObject @object)
    {
        // TODO: Maybe it will be a good idea to somehow pass table's filtering mode here
        // instead of hardcoding it to AND. But it may depend on the use case of the filter.
        return Filters.All(filter => filter.Eval(@object));
    }
    public override void Reset()
    {
        // TODO: This will cause double the amount of events to be emitted.
        foreach (var filter in Filters)
        {
            filter.Reset();
        }
    }
}
