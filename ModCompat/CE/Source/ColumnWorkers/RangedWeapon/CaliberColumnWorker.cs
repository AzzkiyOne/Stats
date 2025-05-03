using System;
using RimWorld;
using Stats.ColumnWorkers;
using Stats.Widgets;
using Stats.Widgets.FilterWidgets;
using Verse;

namespace Stats.ModCompat.CE.ColumnWorkers.RangedWeapon;

public sealed class CaliberColumnWorker : ColumnWorker
{
    private static readonly StatDef CaliberStatDef = DefDatabase<StatDef>.GetNamed("Caliber");
    private static readonly Func<ThingAlike, string> GetCaliberName =
        FunctionExtensions.Memoized((ThingAlike thing) =>
        {
            var statReq = StatRequest.For(thing.Def, thing.StuffDef);

            return CaliberStatDef.Worker.GetStatDrawEntryLabel(
                CaliberStatDef,
                CaliberStatDef.Worker.GetValue(statReq),
                ToStringNumberSense.Absolute,
                statReq
            ) ?? "";
        });
    private CaliberColumnWorker() : base(TableColumnCellStyle.String)
    {
    }
    public static CaliberColumnWorker Make(ColumnDef _) => new();
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        var caliberName = GetCaliberName(thing);

        if (caliberName.Length == 0)
        {
            return null;
        }

        var statReq = StatRequest.For(thing.Def, thing.StuffDef);
        var tooltip = CaliberStatDef.Worker.GetExplanationFull(
            statReq,
            ToStringNumberSense.Absolute,
            CaliberStatDef.Worker.GetValue(statReq)
        );

        return new Label(caliberName).Tooltip(tooltip);
    }
    public override FilterWidget GetFilterWidget()
    {
        return new StringFilter(GetCaliberName);
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return GetCaliberName(thing1).CompareTo(GetCaliberName(thing2));
    }
}
