using RimWorld;
using Stats.ColumnWorkers.Generic;

namespace Stats.ModCompat.CE.ColumnWorkers.RangedWeapon;

public sealed class OneHandednessColumnWorker
    : BooleanColumnWorker
{
    protected override bool GetValue(ThingAlike thing)
    {
        var statReq = StatRequest.For(thing.Def, thing.StuffDef);

        if (ColumnDef.stat!.Worker.ShouldShowFor(statReq) == false)
        {
            return false;
        }

        return ColumnDef.stat!.Worker.GetValue(statReq) > 0f;
    }
}
