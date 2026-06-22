using System.Collections.Generic;
using System.Linq;
using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;
using Stats.TableWorkers;
using Stats.Utils.Extensions;

namespace Stats.ColumnWorkers.PawnDef;

public sealed class WeaponsColumnWorker<TRecord>(ColumnDef columnDef) :
    ThingDefSetColumnWorker<TRecord, ThingDefSetCell>(columnDef)
        where TRecord :
            IThingDefTableRecord
{
    protected override ThingDefSetCell MakeCell(TRecord record)
    {
        Verse.ThingDef thingDef = record.ThingDef;
        HashSet<Verse.ThingDef>? weapons = thingDef.GetPossibleWeapons();

        if (weapons != null)
        {
            return new ThingDefSetCell(weapons);
        }

        return default;
    }

    protected override IEnumerable<Verse.ThingDef?> GetValueFieldFilterOptions(TableWorker tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .SelectMany(thingDef => thingDef.GetPossibleWeapons() ?? [])
            .Distinct();
    }
}
