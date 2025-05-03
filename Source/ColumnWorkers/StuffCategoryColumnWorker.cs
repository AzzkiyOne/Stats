using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Widgets;
using Stats.Widgets.FilterWidgets;
using Verse;

namespace Stats.ColumnWorkers;

public sealed class StuffCategoryColumnWorker : ColumnWorker
{
    public override TableColumnCellStyle CellStyle => TableColumnCellStyle.String;
    private static readonly Func<ThingAlike, HashSet<StuffCategoryDef>> GetStuffCatDefs = FunctionExtensions.Memoized(
        (ThingAlike thing) => thing.Def.stuffProps?.categories.ToHashSet() ?? []
    );
    private static readonly Func<ThingAlike, string> GetStuffCatLabels = FunctionExtensions.Memoized(
        (ThingAlike thing) =>
        {
            var stuffCatDefs = GetStuffCatDefs(thing);

            if (stuffCatDefs.Count == 0)
            {
                return "";
            }

            var labels = stuffCatDefs
                .OrderBy(stuffCatDef => stuffCatDef.label)
                .Select(stuffCatDef => stuffCatDef.LabelCap);

            return string.Join("\n", labels);
        }
    );
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        var stuffCatLabels = GetStuffCatLabels(thing);

        if (stuffCatLabels.Length == 0)
        {
            return null;
        }

        return new Label(stuffCatLabels);
    }
    public override FilterWidget GetFilterWidget()
    {
        return new ManyToManyFilter<StuffCategoryDef>(
            GetStuffCatDefs,
            DefDatabase<StuffCategoryDef>.AllDefsListForReading.OrderBy(stuffCatDef => stuffCatDef.label),
            stuffCatDef => new Label(stuffCatDef.LabelCap)
        );
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return GetStuffCatLabels(thing1).CompareTo(GetStuffCatLabels(thing2));
    }
}
