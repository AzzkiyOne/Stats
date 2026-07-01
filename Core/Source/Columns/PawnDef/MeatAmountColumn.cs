using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Columns.Cells;
using Stats.TableRecords;
using Stats.Tables;
using Stats.Utils.Extensions;
using Verse;

namespace Stats.Columns.PawnDef;

public sealed class MeatAmountColumn<TRecord>(ColumnDef columnDef) :
    ThingDefCountColumn<TRecord, ThingDefCountCell>(columnDef)
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

    protected override IEnumerable<Verse.ThingDef?> GetTypeFieldFilterOptions(Table tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .Select(thingDef => thingDef.race?.meatDef)
            .Distinct();
    }
}
