using System;
using System.Collections.Generic;
using System.Linq;
using Stats.ThingTable;
using Stats.Widgets;
using Verse;

namespace Stats.Compat.Biotech.ThingTable;

public sealed class ValidLifeStageColumnWorker : ColumnWorker<ThingAlike>
{
    private static readonly Func<ThingAlike, HashSet<DevelopmentalStage>> GetValidLifeStages =
    FunctionExtensions.Memoized((ThingAlike thing) =>
    {
        var lifeStages = new HashSet<DevelopmentalStage>();

        if (thing.Def.apparel == null)
        {
            return lifeStages;
        }

        var enumerator = thing.Def.apparel.developmentalStageFilter.Enumerate().GetEnumerator();

        while (enumerator.MoveNext())
        {
            lifeStages.Add(enumerator.Current);
        }

        return lifeStages;
    });
    private static readonly Func<ThingAlike, string> GetLifeStageLabels =
    FunctionExtensions.Memoized((ThingAlike thing) =>
    {
        var lifeStages = GetValidLifeStages(thing);

        if (lifeStages.Count == 0)
        {
            return "";
        }

        var lifeStageLabels = lifeStages
            .OrderBy(lifeStage => lifeStage)
            .Select(lifeStage => lifeStage.ToString().Translate().CapitalizeFirst().RawText);

        return string.Join("\n", lifeStageLabels);
    });
    public ValidLifeStageColumnWorker(ColumnDef columnDef) : base(columnDef, ColumnCellStyle.String)
    {
    }
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        var lifeStageLabels = GetLifeStageLabels(thing);

        if (lifeStageLabels.Length == 0)
        {
            return null;
        }

        return new Label(lifeStageLabels);
    }
    public override FilterWidget<ThingAlike> GetFilterWidget(IEnumerable<ThingAlike> tableRecords)
    {
        var filterOptions = tableRecords
            .SelectMany(GetValidLifeStages)
            .Distinct()
            .OrderBy(lifeStage => lifeStage)
            .Select(lifeStage => new NTMFilterOption<DevelopmentalStage>(
                lifeStage,
                lifeStage.ToString().Translate().CapitalizeFirst().RawText
            ));

        return Make.MTMFilter(GetValidLifeStages, filterOptions);
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return GetLifeStageLabels(thing1).CompareTo(GetLifeStageLabels(thing2));
    }
}
