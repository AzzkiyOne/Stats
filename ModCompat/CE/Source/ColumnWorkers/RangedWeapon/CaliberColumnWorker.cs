using System;
using System.Collections.Generic;
using RimWorld;
using Stats.ColumnWorkers;
using Stats.Widgets;
using Stats.Widgets.FilterWidgets;
using Verse;

namespace Stats.ModCompat.CE.ColumnWorkers.RangedWeapon;

public sealed class CaliberColumnWorker : ColumnWorker
{
    public override TableColumnCellStyle CellStyle => TableColumnCellStyle.String;
    private static readonly StatDef CaliberStatDef = DefDatabase<StatDef>.GetNamed("Caliber");
    private static readonly Func<ThingAlike, string?> GetCaliberName = FunctionExtensions.Memoized(
        (ThingAlike thing) =>
        {
            var statReq = StatRequest.For(thing.Def, thing.StuffDef);

            return CaliberStatDef.Worker.GetStatDrawEntryLabel(
                CaliberStatDef,
                CaliberStatDef.Worker.GetValue(statReq),
                ToStringNumberSense.Absolute,
                statReq
            );
        }
    );
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        var caliberName = GetCaliberName(thing);

        if (caliberName is null or { Length: 0 })
        {
            return null;
        }

        var statReq = StatRequest.For(thing.Def, thing.StuffDef);
        var tooltip = ColumnDef.stat!.Worker.GetExplanationFull(
            statReq,
            ToStringNumberSense.Absolute,
            ColumnDef.stat!.Worker.GetValue(statReq)
        );

        return new Label(caliberName).Tooltip(tooltip);
    }
    public override FilterWidget GetFilterWidget()
    {
        return new StringFilter(GetCaliberName);
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        var caliberName1 = GetCaliberName(thing1);
        var caliberName2 = GetCaliberName(thing2);

        return Comparer<string?>.Default.Compare(caliberName1, caliberName2);
    }
}
