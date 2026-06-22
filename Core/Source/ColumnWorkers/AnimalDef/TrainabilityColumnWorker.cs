using System.Collections.Generic;
using System.Linq;
using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;
using Stats.TableWorkers;
using Verse;

namespace Stats.ColumnWorkers.AnimalDef;

public sealed class TrainabilityColumnWorker<TRecord>(ColumnDef columnDef) :
    DefColumnWorker<TRecord, DefCell>(columnDef)
        where TRecord :
            IPawnDefTableRecord
{
    protected override DefCell MakeCell(TRecord record)
    {
        RaceProperties raceProperties = record.RaceProperties;
        TrainabilityDef? trainability = raceProperties.trainability;

        if (trainability != null)
        {
            return new DefCell(trainability);
        }

        return default;
    }

    protected override IEnumerable<Verse.Def?> GetValueFieldFilterOptions(TableWorker tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .Select(thingDef => thingDef.race?.trainability)
            .Distinct();
    }
}
