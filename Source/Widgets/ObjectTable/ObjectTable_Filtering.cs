using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Stats.Widgets;

internal sealed partial class ObjectTable<TObject>
{
    private readonly HashSet<FilterWidget<TObject>> ActiveFilters;
    private bool ShouldApplyFilters;
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
            ObjectMatchesFilters = value switch
            {
                TableFilterMode.AND => ObjectFilterMatchFuncAND,
                TableFilterMode.OR => ObjectFilterMatchFuncOR,
                _ => throw new NotSupportedException("Unsupported table filtering mode.")
            };

            OnFilterModeChange?.Invoke(value);

            if (ActiveFilters.Count > 1)
            {
                ShouldApplyFilters = true;
            }
        }
    }
    public override event Action<TableFilterMode>? OnFilterModeChange;
    private ObjectFilterMatchFunc ObjectMatchesFilters;
    private static readonly ObjectFilterMatchFunc ObjectFilterMatchFuncAND =
        (@object, filters) => filters.All(filter => filter.Eval(@object));
    private static readonly ObjectFilterMatchFunc ObjectFilterMatchFuncOR =
        (@object, filters) => filters.Any(filter => filter.Eval(@object));
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
        if (ActiveFilters.Count == 0)
        {
            FilteredBodyRows.ResetTo(UnfilteredBodyRows);
        }
        else
        {
            FilteredBodyRows.Clear();

            foreach (var row in UnfilteredBodyRows)
            {
                var rowIsValid = ObjectMatchesFilters(row.Object, ActiveFilters);

                if (rowIsValid)
                {
                    FilteredBodyRows.Add(row);
                }
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
            FilteredBodyRows.ResetTo(UnfilteredBodyRows);
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

    private delegate bool ObjectFilterMatchFunc(TObject @object, HashSet<FilterWidget<TObject>> filters);
}
