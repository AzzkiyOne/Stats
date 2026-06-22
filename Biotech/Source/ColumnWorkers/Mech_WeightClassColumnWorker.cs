using Stats.TableRecords.ThingDef;
using Verse;

namespace Stats.Compat.Biotech;

public sealed class Mech_WeightClassColumnWorker : DefColumnWorker<VirtualThing, MechWeightClassDef>
{
    public Mech_WeightClassColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override MechWeightClassDef GetValue(VirtualThing thing)
    {
        return thing.Def.race.mechWeightClass;
    }
}
