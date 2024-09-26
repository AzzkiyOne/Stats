using CombatExtended;
using RimWorld;
using Verse;

namespace Stats.Compat.CE;

public class Column_Caliber : Table.Columns.Column_Stat
{
    public override Table.ICell? GetCell(ThingAlike thing)
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

        return new Table.Cells.Cell_Gen<string>(cellText, cellText, cellTip);
    }
}

public class Column_ReloadTime : Table.Columns.Column_Stat
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
