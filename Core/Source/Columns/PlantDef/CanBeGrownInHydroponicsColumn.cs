using Stats.TableRecords;

namespace Stats.Columns.PlantDef;

public sealed class CanBeGrownInHydroponicsColumn<TRecord>(ColumnDef columnDef) :
    BooleanColumn<TRecord>(columnDef)
        where TRecord :
            IPlantDefTableRecord
{
    protected override bool GetValue(TRecord record)
    {
        return record.PlantProperties.sowTags.Contains("Hydroponic");
    }
}
