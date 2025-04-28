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
    private static readonly Func<ThingAlike, List<ApparelLayerDef>> GetApparelLayers = FunctionExtensions.Memoized(
        (ThingAlike thing) => thing.Def.apparel?.layers ?? []
    );
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        var layerDefs = GetApparelLayers(thing);

        if (layerDefs.Count == 0)
        {
            return null;
        }

        var layerLabels = layerDefs
            .OrderBy(layerDef => layerDef.label)
            .Select(layerDef => layerDef.LabelCap);

        return new Label(string.Join("\n", layerLabels));
    }
    public override FilterWidget GetFilterWidget()
    {
        return new ManyToManyOptionsFilter<ApparelLayerDef>(
            GetApparelLayers,
            DefDatabase<ApparelLayerDef>.AllDefsListForReading.OrderBy(layerDef => layerDef.label),
            layerDef => new Label(layerDef.LabelCap)
        );
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        var layersCount1 = GetApparelLayers(thing1).Count;
        var layersCount2 = GetApparelLayers(thing2).Count;

        return layersCount1.CompareTo(layersCount2);
    }
}
