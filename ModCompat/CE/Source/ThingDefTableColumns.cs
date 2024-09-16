using CombatExtended;
using RimWorld;
using Stats.ThingDefTable;
using Verse;

namespace Stats.Compat.CE;

public class Column_Caliber : Column_Stat
{
    public override GenTable.ICell? GetCell(ThingAlike thing)
    {
        var statReq = StatRequest.For(thing.Def, thing.Stuff);

        if (stat.Worker.ShouldShowFor(statReq) == false)
        {
            return null;
        }

        var cellText = stat.Worker.GetStatDrawEntryLabel(
            stat,
            stat.Worker.GetValue(statReq),
            ToStringNumberSense.Absolute,
            StatRequest.For(thing.Def, thing.Stuff)
        );
        var cellTip = stat.Worker.GetExplanationFull(
            statReq,
            ToStringNumberSense.Absolute,
            stat.Worker.GetValue(statReq)
        );

        return new GenTable.Cell_Str(cellText, cellTip);
    }
}

public class Column_ReloadTime : Column_Stat
{
    protected override float? GetValue(ThingAlike thing)
    {
        var statReq = StatRequest.For(thing.Def, thing.Stuff);

        if (CE_StatDefOf.MagazineCapacity.Worker.ShouldShowFor(statReq) == false)
        {
            return null;
        }

        return stat.Worker.GetValue(statReq);
    }
}