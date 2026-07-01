using Stats.TableRecords;

namespace Stats.Columns.AnimalDef;

public sealed class IsPackAnimalColumn<TRecord>(ColumnDef columnDef) :
    BooleanColumn<TRecord>(columnDef)
        where TRecord :
            IPawnDefTableRecord
{
    protected override bool GetValue(TRecord record)
    {
        return record.RaceProperties.packAnimal;
    }
}
