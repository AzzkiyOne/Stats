using Stats.TableRecords;

namespace Stats.Columns.ApparelDef;

public sealed class CountsAsClothingForNudityColumn<TRecord>(ColumnDef columnDef) :
    BooleanColumn<TRecord>(columnDef)
        where TRecord :
            IApparelDefTableRecord
{
    protected override bool GetValue(TRecord record)
    {
        return record.ApparelProperties.countsAsClothingForNudity;
    }
}
