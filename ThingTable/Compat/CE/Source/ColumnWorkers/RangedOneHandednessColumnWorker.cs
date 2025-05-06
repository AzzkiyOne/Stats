using System;
using CombatExtended;
using RimWorld;

namespace Stats.ThingTable.Compat.CE;

public static class RangedOneHandednessColumnWorker
{
    public static BooleanColumnWorker<ThingAlike> Make(ColumnDef _) => new(IsOneHandedWeapon.Memoized());
    public static readonly Func<ThingAlike, bool> IsOneHandedWeapon = thing =>
    {
        var statReq = StatRequest.For(thing.Def, thing.StuffDef);

        if (CE_StatDefOf.OneHandedness.Worker.ShouldShowFor(statReq))
        {
            return CE_StatDefOf.OneHandedness.Worker.GetValue(statReq) > 0f;
        }

        return false;
    };
}
