using System.Linq;

namespace Stats.ThingTable.Compat.CE;

public sealed class MeleeSharpArmorPenetrationColumnWorker : StatColumnWorker
{
    public MeleeSharpArmorPenetrationColumnWorker()
        : base(StatDefOf.MeleeWeapon_AverageArmorPenetration, StatValueExplanationType.Full)
    {
    }
    new public static MeleeSharpArmorPenetrationColumnWorker Make(ColumnDef _) => new();
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        return base.GetStatDrawEntryLabel(thing).Split(',').First();
    }
}
