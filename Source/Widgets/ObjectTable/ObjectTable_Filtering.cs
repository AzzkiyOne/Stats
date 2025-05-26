using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Stats.Widgets;

internal sealed partial class ObjectTable<TObject>
{
    private readonly HashSet<FilterWidget<TObject>> ActiveFilters;
    private bool ShouldApplyFilters = false;
    private TableFilterMode _FilterMode;
    public override TableFilterMode FilterMode
    {
        get => _FilterMode;
        set
        {
            if (value == _FilterMode)
            {
                return;
            }

            _FilterMode = value;
            OnFilterModeChange?.Invoke(value);

            if (ActiveFilters.Count > 1)
            {
                ShouldApplyFilters = true;
            }
        }
    }
    public override event Action<TableFilterMode>? OnFilterModeChange;
    private void HandleFilterChange(FilterWidget<TObject> filter, Column column)
    {
        if (filter.IsActive)
        {
            ActiveFilters.Add(filter);

            // We do not check whether the filter was added to active filters
            // because we have to adjust its column width regardless.
            var filterWidth = filter.GetSize().x;

            column.Width = Mathf.Max(column.InitialWidth, filterWidth);
        }
        else
        {
            var filterWasRemoved = ActiveFilters.Remove(filter);

            if (filterWasRemoved)
            {
                column.Width = column.InitialWidth;
            }
        }

        ShouldApplyFilters = true;
    }
    private void ApplyFilters()
    {
        if (FilterMode == TableFilterMode.AND)
        {
            foreach (var row in BodyRows)
            {
                row.IsHidden = ActiveFilters.Any(filter => filter.Eval(row.Object) == false);
            }
        }
        else
        {
            foreach (var row in BodyRows)
            {
                row.IsHidden = !ActiveFilters.Any(filter => filter.Eval(row.Object));
            }
        }

        ShouldApplyFilters = false;
    }
    public override void ResetFilters()
    {
        if (ActiveFilters.Count == 0)
        {
            return;
        }

        // We have to copy the thing because resetting a filter will remove it from
        // original collection, which will cause an exception, if we were to iterate
        // over it at the same time. Hope compiler/JIT will optimize this.
        foreach (var filter in ActiveFilters.ToArray())
        {
            filter.Reset();
        }

        if (ActiveFilters.Count == 0)
        {
            foreach (var row in BodyRows)
            {
                row.IsHidden = false;
            }
        }
        else
        {
            ShouldApplyFilters = true;
        }
    }
    public override void ToggleFilterMode()
    {
        FilterMode = FilterMode switch
        {
            TableFilterMode.AND => TableFilterMode.OR,
            TableFilterMode.OR => TableFilterMode.AND,
            _ => throw new NotSupportedException("Unsupported table filtering mode."),
        };
    }
}
