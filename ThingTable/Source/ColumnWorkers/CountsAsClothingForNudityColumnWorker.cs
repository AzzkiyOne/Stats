namespace Stats.ThingTable;

public sealed class CountsAsClothingForNudityColumnWorker : BooleanColumnWorker<ThingAlike>
{
    public CountsAsClothingForNudityColumnWorker(ColumnDef columnDef) : base(columnDef, false)
    {
    }
    protected override bool GetValue(ThingAlike thing)
    {
        return thing.Def.apparel?.countsAsClothingForNudity ?? false;
    }
}
