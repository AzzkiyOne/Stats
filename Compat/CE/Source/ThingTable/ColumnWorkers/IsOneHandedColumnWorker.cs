using CombatExtended;
using RimWorld;
using Stats.ThingTable;

namespace Stats.Compat.CE.ThingTable;

public sealed class IsOneHandedColumnWorker : BooleanColumnWorker<ThingAlike>
{
    public IsOneHandedColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetValue(ThingAlike thing)
    {
        var statReq = StatRequest.For(thing.Def, thing.StuffDef);

        if (CE_StatDefOf.OneHandedness.Worker.ShouldShowFor(statReq))
        {
            return CE_StatDefOf.OneHandedness.Worker.GetValue(statReq) > 0f;
        }

        return false;
    }
}
