namespace Stats;

public abstract class ColumnWorker_Str : ColumnWorker<string?>
{
    public override ColumnCellStyle CellStyle => ColumnCellStyle.String;
    protected override bool ShouldShowValue(string? value)
    {
        return value?.Length > 0;
    }
    protected override IWidget GetTableCellContent(string? value, ThingRec thing)
    {
        return new Widget_Label_Temp(value!);
    }
    public override IWidget_FilterInput GetFilterWidget()
    {
        return new Widget_FilterInput_Str(GetValue);
    }
    public override int Compare(ThingRec thing1, ThingRec thing2)
    {
        var v1 = GetValue(thing1);
        var v2 = GetValue(thing2);

        if (v1 == null)
        {
            if (v2 == null) return 0;

            return -1;
        }

        return v1.CompareTo(v2);
    }
}
