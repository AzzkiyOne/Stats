using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Widgets;
using Verse;

namespace Stats.ThingTable;

public sealed class StuffCategoryColumnWorker : ColumnWorker<ThingAlike>
{
    private static readonly Func<ThingAlike, HashSet<StuffCategoryDef>> GetStuffCatDefs =
        FunctionExtensions.Memoized((ThingAlike thing) =>
        {
            return thing.Def.stuffProps?.categories.ToHashSet() ?? [];
        });
    private static readonly Func<ThingAlike, string> GetStuffCatLabels =
        FunctionExtensions.Memoized((ThingAlike thing) =>
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
        });
    private StuffCategoryColumnWorker() : base(TableColumnCellStyle.String)
    {
    }
    public static StuffCategoryColumnWorker Make(ColumnDef _) => new();
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        var stuffCatLabels = GetStuffCatLabels(thing);

        if (stuffCatLabels.Length == 0)
        {
            return null;
        }

        return new Label(stuffCatLabels);
    }
    public override FilterWidget<ThingAlike> GetFilterWidget(IEnumerable<ThingAlike> tableRecords)
    {
        var stuffCatDefs = tableRecords
            .SelectMany(GetStuffCatDefs)
            .Distinct()
            .OrderBy(stuffCatDef => stuffCatDef.label);

        return new ManyToManyFilter<ThingAlike, StuffCategoryDef>(
            GetStuffCatDefs,
            stuffCatDefs,
            stuffCatDef => new Label(stuffCatDef.LabelCap)
        );
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return GetStuffCatLabels(thing1).CompareTo(GetStuffCatLabels(thing2));
    }
}
