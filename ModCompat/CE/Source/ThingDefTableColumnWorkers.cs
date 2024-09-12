using CombatExtended;
using RimWorld;
using Stats.ThingDefTable;
using Verse;

namespace Stats.Compat.CE;

public class ColumnWorker_Caliber : ColumnWorker
{
    public override GenTable.Cell? GetCell(ThingAlike thing)
    {
        var stat = DefDatabase<StatDef>.GetNamed("Caliber");
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

public class ColumnWorker_ReloadTime : ColumnWorker
{
    public override GenTable.Cell? GetCell(ThingAlike thing)
    {
        var stat = CE_StatDefOf.ReloadTime;
        var statReq = StatRequest.For(thing.Def, thing.Stuff);

        if (CE_StatDefOf.MagazineCapacity.Worker.ShouldShowFor(statReq) == false)
        {
            return null;
        }

        var statValue_Num = stat.Worker.GetValue(statReq);
        var statValue_Str = stat.Worker.GetStatDrawEntryLabel(
            stat,
            statValue_Num,
            ToStringNumberSense.Absolute,
            StatRequest.For(thing.Def, thing.Stuff)
        );

        return new GenTable.Cell_Num(statValue_Num, statValue_Str);
    }
}