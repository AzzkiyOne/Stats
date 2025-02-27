namespace Stats;

public abstract class ColumnWorker_Bool : ColumnWorker<bool>
{
    public override ColumnCellStyle CellStyle => ColumnCellStyle.Boolean;
    protected override bool ShouldShowValue(bool value)
    {
        return value == true;
    }
    protected override ICellWidget ValueToCellWidget(bool value, ThingRec thing)
    {
        return new CellWidget_Bool(value);
    }
    public override IFilterWidget GetFilterWidget()
    {
        return new FilterWidget_Bool(GetValue);
    }
    public override int Compare(ThingRec thing1, ThingRec thing2)
    {
        return GetValue(thing1).CompareTo(GetValue(thing2));
    }
}
