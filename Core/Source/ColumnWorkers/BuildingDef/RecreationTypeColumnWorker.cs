using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.BuildingDef;

public sealed class RecreationTypeColumnWorker<TRecord>(ColumnDef columnDef) :
    DefColumnWorker<TRecord, DefCell>(columnDef)
        where TRecord :
            IBuildingDefTableRecord
{
    protected override DefCell MakeCell(TRecord record)
    {
        BuildingProperties buildingProperties = record.BuildingProperties;
        JoyKindDef? joyKind = buildingProperties.joyKind;

        if (joyKind != null)
        {
            return new DefCell(joyKind);
        }

        return default;
    }

    protected override IEnumerable<Verse.Def?> GetValueFieldFilterOptions(TableWorker tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .Select(thingDef => thingDef.building?.joyKind)
            .Distinct();
    }
}
