using RimWorld;

namespace Stats.Compat.CE;

public class ColumnWorker_WeaponRanged_OneHandedness
    : ColumnWorker_Bool
{
    public override bool GetValue(ThingRec thing)
    {
        var statReq = StatRequest.For(thing.Def, thing.StuffDef);

        if (ColumnDef.stat!.Worker.ShouldShowFor(statReq) == false) return false;

        return ColumnDef.stat!.Worker.GetValue(statReq) > 0f;
    }
}
