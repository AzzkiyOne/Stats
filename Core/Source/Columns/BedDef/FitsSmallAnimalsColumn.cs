using Stats.TableRecords;

namespace Stats.Columns.BedDef;

public sealed class FitsSmallAnimalsColumn<TRecord>(ColumnDef columnDef) :
    BooleanColumn<TRecord>(columnDef)
        where TRecord :
            IBuildingDefTableRecord
{
    protected override bool GetValue(TRecord record)
    {
        return record.BuildingProperties.bed_humanlike == false;
    }
}
