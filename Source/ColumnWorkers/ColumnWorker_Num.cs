namespace Stats;

public abstract class ColumnWorker_Num : ColumnWorker<float>
{
    public override ColumnCellStyle CellStyle => ColumnCellStyle.Number;
    protected virtual string FormatValue(float value)
    {
        if (ColumnDef.formatString != null)
        {
            return value.ToString(ColumnDef.formatString);
        }

        return value.ToString();
    }
    protected override bool ShouldShowValue(float value)
    {
        return value != 0f && float.IsNaN(value) == false;
    }
    protected override ICellWidget ValueToCellWidget(float value, ThingRec thing)
    {
        var valueStr = FormatValue(value);

        return new CellWidget_Str(valueStr);
    }
    public override IFilterWidget GetFilterWidget()
    {
        return new FilterWidget_Num(GetValue);
    }
    public override int Compare(ThingRec thing1, ThingRec thing2)
    {
        return GetValue(thing1).CompareTo(GetValue(thing2));
    }
    public float? GetCellValue(ThingRec thing)
    {
        return GetValue(thing);
    }
}
