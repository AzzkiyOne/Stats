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
    protected override Widget GetTableCellContent(float value, ThingRec thing)
    {
        var valueStr = FormatValue(value);

        return new Widget_Label(valueStr)
        {
            Width = 100,
        };
    }
    public override IWidget_FilterInput GetFilterWidget()
    {
        return new Widget_FilterInput_Num(GetValue);
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
