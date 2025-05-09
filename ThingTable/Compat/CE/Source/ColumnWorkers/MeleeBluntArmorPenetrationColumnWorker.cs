using System.Linq;

namespace Stats.ThingTable.Compat.CE;

public sealed class MeleeBluntArmorPenetrationColumnWorker : StatColumnWorker
{
    public MeleeBluntArmorPenetrationColumnWorker()
        : base(StatDefOf.MeleeWeapon_AverageArmorPenetration, StatValueExplanationType.Full)
    {
    }
    new public static MeleeBluntArmorPenetrationColumnWorker Make(ColumnDef _) => new();
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        return base.GetStatDrawEntryLabel(thing).Split(',').Last();
    }
}
