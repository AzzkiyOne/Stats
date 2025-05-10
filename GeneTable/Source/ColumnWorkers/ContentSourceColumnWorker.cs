using Verse;

namespace Stats.GeneTable;

public sealed class ContentSourceColumnWorker : Stats.ContentSourceColumnWorker<GeneDef>
{
    private ContentSourceColumnWorker() : base(false)
    {
    }
    public static ContentSourceColumnWorker Make(ColumnDef _) => new();
    protected override ModContentPack? GetModContentPack(GeneDef geneDef)
    {
        return geneDef.modContentPack;
    }
}
