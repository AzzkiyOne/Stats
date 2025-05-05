namespace Stats.ThingTable;

public static class ContentSourceColumnWorker
{
    public static ContentSourceColumnWorker<ThingAlike> Make(ColumnDef _) => new(thing => thing.Def.modContentPack);
}
