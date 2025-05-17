using System;
using System.Collections.Generic;
using System.Linq;
using Stats.Widgets;
using Verse;

namespace Stats.ThingTable;

public sealed class OccupiedApparelLayersColumnWorker : ColumnWorker<ThingAlike>
{
    private static readonly Func<ThingAlike, HashSet<ApparelLayerDef>> GetLayerDefs =
    FunctionExtensions.Memoized((ThingAlike thing) =>
    {
        return thing.Def.apparel?.layers.ToHashSet() ?? [];
    });
    private static readonly Func<ThingAlike, string> GetLayersLabels =
    FunctionExtensions.Memoized((ThingAlike thing) =>
    {
        var layerDefs = GetLayerDefs(thing);

        if (layerDefs.Count == 0)
        {
            return "";
        }

        var layerLabels = layerDefs
            .OrderBy(layerDef => layerDef.label)
            .Select(layerDef => layerDef.LabelCap);

        return string.Join("\n", layerLabels);
    });
    public OccupiedApparelLayersColumnWorker(ColumnDef columnDef) : base(columnDef, ColumnCellStyle.String)
    {
    }
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        var layerLabels = GetLayersLabels(thing);

        if (layerLabels.Length == 0)
        {
            return null;
        }

        return new Label(layerLabels);
    }
    public override FilterWidget<ThingAlike> GetFilterWidget(IEnumerable<ThingAlike> tableRecords)
    {
        var layerDefs = tableRecords
            .SelectMany(GetLayerDefs)
            .Distinct()
            .OrderBy(layerDef => layerDef.label);

        return new ManyToManyFilter<ThingAlike, ApparelLayerDef>(
            GetLayerDefs,
            layerDefs,
            layerDef => new Label(layerDef.LabelCap)
        );
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return GetLayersLabels(thing1).CompareTo(GetLayersLabels(thing2));
    }
}
