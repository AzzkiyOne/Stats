namespace Stats.ThingTable;

public sealed class CountsAsClothingForNudityColumnWorker : BooleanColumnWorker<ThingAlike>
{
    private CountsAsClothingForNudityColumnWorker() : base(false)
    {
    }
    public static CountsAsClothingForNudityColumnWorker Make(ColumnDef _) => new();
    protected override bool GetValue(ThingAlike thing)
    {
        return thing.Def.apparel?.countsAsClothingForNudity ?? false;
    }
}
