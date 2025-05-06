using System;

namespace Stats.ThingTable;

public static class CountsAsClothingForNudityColumnWorker
{
    public static BooleanColumnWorker<ThingAlike> Make(ColumnDef _) => new(GetValue);
    public static readonly Func<ThingAlike, bool> GetValue = thing =>
    {
        return thing.Def.apparel?.countsAsClothingForNudity ?? false;
    };
}
