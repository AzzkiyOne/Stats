using Verse;

namespace Stats.ThingTable;

public sealed class ContentSourceColumnWorker : Stats.ContentSourceColumnWorker<ThingAlike>
{
    private ContentSourceColumnWorker() : base(false)
    {
    }
    public static ContentSourceColumnWorker Make(ColumnDef _) => new();
    protected override ModContentPack? GetModContentPack(ThingAlike thing)
    {
        return thing.Def.modContentPack;
    }
}
