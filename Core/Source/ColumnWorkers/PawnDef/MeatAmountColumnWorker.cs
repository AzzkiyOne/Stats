using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;
using Stats.TableWorkers;
using Stats.Utils.Extensions;
using Verse;

namespace Stats.ColumnWorkers.PawnDef;

public sealed class MeatAmountColumnWorker<TRecord>(ColumnDef columnDef) :
    ThingDefCountColumnWorker<TRecord, ThingDefCountCell>(columnDef)
        where TRecord :
            IPawnDefTableRecord
{
    protected override ThingDefCountCell MakeCell(TRecord record)
    {
        RaceProperties raceProperties = record.RaceProperties;
        Verse.ThingDef? meatDef = raceProperties.meatDef;

        if (meatDef != null)
        {
            Verse.ThingDef thingDef = record.ThingDef;
            float meatAmount = thingDef.GetStatValuePerceived(StatDefOf.MeatAmount);

            if (meatAmount > 0f)
            {
                return new ThingDefCountCell(meatDef, meatAmount);
            }
        }

        return default;
    }

    protected override IEnumerable<Verse.ThingDef?> GetTypeFieldFilterOptions(TableWorker tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .Select(thingDef => thingDef.race?.meatDef)
            .Distinct();
    }
}
