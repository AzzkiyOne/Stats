using CombatExtended;
using Stats.TableRecords.ThingDef;

namespace Stats.Compat.CE;

public sealed class Weapon_IsOneHandedColumnWorker : BooleanColumnWorker<VirtualThing>
{
    public Weapon_IsOneHandedColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetCellValue(VirtualThing thing)
    {
        return thing.Def.GetStatValuePerceived(CE_StatDefOf.OneHandedness, thing.StuffDef) > 0f;
    }
}
