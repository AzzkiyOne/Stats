using System.Linq;
using CombatExtended;

namespace Stats.ThingTable.Compat.CE;

public sealed class RangedMagazineCapacityColumnWorker : StatColumnWorker
{
    public RangedMagazineCapacityColumnWorker() : base(CE_StatDefOf.MagazineCapacity)
    {
    }
    new public static RangedMagazineCapacityColumnWorker Make(ColumnDef _) => new();
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        return base.GetStatDrawEntryLabel(thing).Split('/').First();
    }
}
