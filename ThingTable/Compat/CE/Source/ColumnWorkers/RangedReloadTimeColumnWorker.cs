using System.Linq;
using CombatExtended;

namespace Stats.ThingTable.Compat.CE;

public sealed class RangedReloadTimeColumnWorker : StatColumnWorker
{
    public RangedReloadTimeColumnWorker() : base(CE_StatDefOf.MagazineCapacity)
    {
    }
    new public static RangedReloadTimeColumnWorker Make(ColumnDef _) => new();
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        return base.GetStatDrawEntryLabel(thing).Split('/').Last();
    }
}
