using Stats.ThingTable;
using Verse;

namespace Stats.Compat.Biotech.ThingTable;

public sealed class Mech_RechargerNeededColumnWorker : ThingDefColumnWorker<ThingAlike, ThingDef?>
{
    public Mech_RechargerNeededColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override ThingDef? GetValue(ThingAlike thing)
    {
        return MechanitorUtility.RechargerForMech(thing.Def);
    }
}
