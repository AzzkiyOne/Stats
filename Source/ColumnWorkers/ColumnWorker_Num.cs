namespace Stats;

public abstract class ColumnWorker_Num
    : ColumnWorker<float>
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
    protected override IWidget GetTableCellContent(float value, ThingRec thing)
    {
        var valueStr = FormatValue(value);

        return new Widget_Label(valueStr);
    }
    public override IWidget_FilterInput GetFilterWidget()
    {
        return new Widget_FilterInput_Num(new(GetValueCached));
    }
    public override int Compare(ThingRec thing1, ThingRec thing2)
    {
        return GetValueCached(thing1).CompareTo(GetValueCached(thing2));
    }
}
