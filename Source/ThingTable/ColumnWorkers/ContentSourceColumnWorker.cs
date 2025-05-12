using Verse;

namespace Stats.ThingTable;

public sealed class ContentSourceColumnWorker : ContentSourceColumnWorker<ThingAlike>
{
    public ContentSourceColumnWorker(ColumnDef columnDef) : base(columnDef, false)
    {
    }
    protected override ModContentPack? GetModContentPack(ThingAlike thing)
    {
        return thing.Def.modContentPack;
    }
}
