using CombatExtended;
using Stats.Extensions;
using Stats.TableRecords.ThingDef;
using Stats.ObjectTable.ColumnWorkers;
using Stats.Utils.Extensions;

namespace Stats.Compat.CE;

public sealed class Weapon_MagazineCapacityColumnWorker : NumberColumnWorker<VirtualThing>
{
    public Weapon_MagazineCapacityColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override decimal GetCellValueSource(VirtualThing thing)
    {
        var thingDef = thing.Def.building?.turretGunDef ?? thing.Def;

        return thingDef.GetStatValuePerceived(CE_StatDefOf.MagazineCapacity).ToDecimal(0);
    }
}
