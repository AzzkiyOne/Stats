using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ApparelDef;

public sealed class LayersColumnWorker<TRecord>(ColumnDef columnDef) :
    DefSetColumnWorker<TRecord, DefSetCell>(columnDef)
        where TRecord :
            IApparelDefTableRecord
{
    protected override DefSetCell MakeCell(TRecord record)
    {
        ApparelProperties apparelProps = record.ApparelProperties;
        List<Verse.ApparelLayerDef> layers = apparelProps.layers;

        return new DefSetCell(layers);
    }

    protected override IEnumerable<Verse.Def?> GetValueFieldFilterOptions(TableWorker tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .SelectMany(thingDef => thingDef.apparel?.layers)
            .Distinct();
    }
}
