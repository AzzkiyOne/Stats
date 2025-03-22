namespace Stats;

public class ColumnWorker_ContentSource : ColumnWorker_Str
{
    public override string? GetValue(ThingRec thing)
    {
        return thing.Def.modContentPack?.Name;
    }
    protected override Widget GetTableCellContent(string? value, ThingRec thing)
    {
        return new Widget_Label(value!)
        {
            Width = 100,
            Tooltip = thing.Def.modContentPack.PackageIdPlayerFacing,
        };
    }
}
