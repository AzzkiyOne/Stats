using RimWorld;
using Stats.ColumnWorkers.Generic;
using Stats.Widgets;
using Verse;

namespace Stats.ModCompat.CE.ColumnWorkers.RangedWeapon;

public sealed class CaliberColumnWorker
    : StringColumnWorker
{
    protected override string GetValue(ThingAlike thing)
    {
        var statReq = StatRequest.For(thing.Def, thing.StuffDef);

        return ColumnDef.stat!.Worker.GetStatDrawEntryLabel(
            ColumnDef.stat!,
            ColumnDef.stat!.Worker.GetValue(statReq),
            ToStringNumberSense.Absolute,
            statReq
        );
    }
    protected override IWidget GetTableCellContent(string? value, ThingAlike thing)
    {
        var statReq = StatRequest.For(thing.Def, thing.StuffDef);
        var tooltip = ColumnDef.stat!.Worker.GetExplanationFull(
            statReq,
            ToStringNumberSense.Absolute,
            ColumnDef.stat!.Worker.GetValue(statReq)
        );

        return new Label(value).Tooltip(tooltip);
    }
}
