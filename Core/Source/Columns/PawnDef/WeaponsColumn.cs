using System.Collections.Generic;
using System.Linq;
using Stats.Columns.Cells;
using Stats.TableRecords;
using Stats.Tables;
using Stats.Utils.Extensions;

namespace Stats.Columns.PawnDef;

public sealed class WeaponsColumn<TRecord>(ColumnDef columnDef) :
    ThingDefSetColumn<TRecord, ThingDefSetCell>(columnDef)
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

    protected override IEnumerable<Verse.ThingDef?> GetValueFieldFilterOptions(Table tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .SelectMany(thingDef => thingDef.GetPossibleWeapons() ?? [])
            .Distinct();
    }
}
