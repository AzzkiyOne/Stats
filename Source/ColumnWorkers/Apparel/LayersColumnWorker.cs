using System.Collections.Generic;
using System.Linq;
using Stats.Widgets;
using Stats.Widgets.FilterWidgets;
using Verse;

namespace Stats.ColumnWorkers.Apparel;

public sealed class LayersColumnWorker : ColumnWorker<IEnumerable<ApparelLayerDef>>
{
    public override TableColumnCellStyle CellStyle => TableColumnCellStyle.String;
    protected override IEnumerable<ApparelLayerDef> GetValue(ThingAlike thing)
    {
        return thing.Def.apparel?.layers ?? [];
    }
    protected override Widget GetTableCellContent(IEnumerable<ApparelLayerDef> layerDefs, ThingAlike thing)
    {
        var layerLabels = layerDefs
            .OrderBy(layerDef => layerDef.label)
            .Select(layerDef => layerDef.LabelCap);

        return new Label(string.Join("\n", layerLabels));
    }
    public override FilterWidget GetFilterWidget()
    {
        return new EnumerableFilter<ApparelLayerDef>(
            GetValueCached,
            DefDatabase<ApparelLayerDef>.AllDefsListForReading.OrderBy(layerDef => layerDef.label),
            layerDef => new Label(layerDef.LabelCap)
        );
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return GetValueCached(thing1).Count().CompareTo(GetValueCached(thing2).Count());
    }
}
