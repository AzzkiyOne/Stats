using Stats.TableRecords.ThingDef;
using Verse;

namespace Stats.Compat.Biotech;

public sealed class Mech_RechargerNeededColumnWorker : ThingDefColumnWorker<VirtualThing, ThingDef?>
{
    public Mech_RechargerNeededColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override ThingDef? GetValue(VirtualThing thing)
    {
        return MechanitorUtility.RechargerForMech(thing.Def);
    }
}
