namespace Stats;

public class ColumnWorker_Name : ColumnWorker_Str
{
    public override string? GetValue(ThingRec thing)
    {
        return thing.StuffDef == null
            ? thing.Def.LabelCap
            : $"{thing.Def.LabelCap} ({thing.StuffDef.LabelCap})";
    }
    protected override ICellWidget ValueToCellWidget(string value, ThingRec thing)
    {
        return new CellWidget_Str(
            value,
            thing.Def.description,
            new ThingIconWidget(thing)
        );
    }
}
