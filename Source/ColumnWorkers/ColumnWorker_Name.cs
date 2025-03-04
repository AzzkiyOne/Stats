namespace Stats;

public class ColumnWorker_Name : ColumnWorker_Str
{
    public override string? GetValue(ThingRec thing)
    {
        return thing.StuffDef == null
            ? thing.Def.LabelCap
            : $"{thing.Def.LabelCap} ({thing.StuffDef.LabelCap})";
    }
    protected override IWidget GetTableCellContent(string? value, ThingRec thing)
    {
        return new Widget_Label_Temp(
            value!,
            thing.Def.description,
            new Widget_ThingIcon(thing)
        );
    }
}
