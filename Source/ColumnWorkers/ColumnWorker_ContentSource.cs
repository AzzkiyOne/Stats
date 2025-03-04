namespace Stats;

public class ColumnWorker_ContentSource : ColumnWorker_Str
{
    public override string? GetValue(ThingRec thing)
    {
        return thing.Def.modContentPack?.Name;
    }
    protected override IWidget GetTableCellContent(string? value, ThingRec thing)
    {
        return new Widget_Label_Temp(
            value!,
            thing.Def.modContentPack.PackageIdPlayerFacing
        );
    }
}
