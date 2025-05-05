using Verse;

namespace Stats.GeneTable;

public static class ContentSourceColumnWorker
{
    public static ContentSourceColumnWorker<GeneDef> Make(ColumnDef _) => new(geneDef => geneDef.modContentPack);
}
