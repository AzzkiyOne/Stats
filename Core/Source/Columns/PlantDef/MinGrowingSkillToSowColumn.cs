using RimWorld;
using Stats.Columns.Cells;
using Stats.TableRecords;

namespace Stats.Columns.PlantDef;

public sealed class MinGrowingSkillToSowColumn<TRecord>(ColumnDef columnDef) :
    NumberColumn<TRecord, NumberCell>(columnDef)
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
