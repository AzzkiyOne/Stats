using System;
using System.Collections.Generic;
using System.Linq;
using Stats.Widgets;
using Stats.Widgets.FilterWidgets;
using Verse;

namespace Stats.ColumnWorkers.Apparel;

public sealed class LayersColumnWorker : ColumnWorker
{
    public override TableColumnCellStyle CellStyle => TableColumnCellStyle.String;
    private static readonly Func<ThingAlike, List<ApparelLayerDef>> GetLayerDefs = FunctionExtensions.Memoized(
        (ThingAlike thing) => thing.Def.apparel?.layers ?? []
    );
    private static readonly Func<ThingAlike, string> GetLayersLabels = FunctionExtensions.Memoized(
        (ThingAlike thing) =>
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
        }
    );
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        var layerLabels = GetLayersLabels(thing);

        if (layerLabels.Length == 0)
        {
            return null;
        }

        return new Label(layerLabels);
    }
    public override FilterWidget GetFilterWidget()
    {
        return new ManyToManyOptionsFilter<ApparelLayerDef>(
            GetLayerDefs,
            DefDatabase<ApparelLayerDef>
                .AllDefsListForReading
                .OrderBy(layerDef => layerDef.label),
            layerDef => new Label(layerDef.LabelCap)
        );
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return GetLayersLabels(thing1).CompareTo(GetLayersLabels(thing2));
    }
}
