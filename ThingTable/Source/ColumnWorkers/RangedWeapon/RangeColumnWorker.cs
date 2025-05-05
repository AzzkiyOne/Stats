using Stats.ColumnWorkers;
using Stats.ThingTable.Defs;

namespace Stats.ThingTable.ColumnWorkers.RangedWeapon;

public static class RangeColumnWorker
{
    public static NumberColumnWorker<ThingAlike> Make(ColumnDef _) =>
        new(thing => thing.Def.Verbs.Primary()?.range.ToDecimal() ?? 0m);
}
