namespace Stats;

public class ColumnWorker_ContentSource : ColumnWorker_Str
{
    public override string? GetValue(ThingRec thing)
    {
        return thing.Def.modContentPack?.Name;
    }
    protected override ICellWidget ValueToCellWidget(string value, ThingRec thing)
    {
        return new CellWidget_Str(
            value,
            thing.Def.modContentPack.PackageIdPlayerFacing
        );
    }
}
