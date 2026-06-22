using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;
using Stats.TableWorkers;
using Stats.Utils.Extensions;

namespace Stats.ColumnWorkers.PawnDef;

public sealed class LeatherAmountColumnWorker<TRecord>(ColumnDef columnDef) :
    ThingDefCountColumnWorker<TRecord, ThingDefCountCell>(columnDef)
        where TRecord :
            IPawnDefTableRecord
{
    protected override ThingDefCountCell MakeCell(TRecord record)
    {
        Verse.ThingDef? leatherDef = record.RaceProperties.leatherDef;

        if (leatherDef != null)
        {
            Verse.ThingDef thingDef = record.ThingDef;
            float leatherAmount = thingDef.GetStatValuePerceived(StatDefOf.LeatherAmount);

            if (leatherAmount > 0f)
            {
                return new ThingDefCountCell(leatherDef, leatherAmount);
            }
        }

        return default;
    }

    protected override IEnumerable<Verse.ThingDef?> GetTypeFieldFilterOptions(TableWorker tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .Select(thingDef => thingDef.race?.leatherDef)
            .Distinct();
    }
}
