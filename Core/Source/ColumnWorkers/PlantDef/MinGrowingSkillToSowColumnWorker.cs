using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;

namespace Stats.ColumnWorkers.PlantDef;

public sealed class MinGrowingSkillToSowColumnWorker<TRecord>(ColumnDef columnDef) :
    NumberColumnWorker<TRecord, NumberCell>(columnDef)
        where TRecord :
            IPlantDefTableRecord
{
    protected override NumberCell MakeCell(TRecord record)
    {
        PlantProperties plantProperties = record.PlantProperties;
        decimal sowMinSkill = plantProperties.sowMinSkill;

        return new NumberCell(sowMinSkill);
    }
}
