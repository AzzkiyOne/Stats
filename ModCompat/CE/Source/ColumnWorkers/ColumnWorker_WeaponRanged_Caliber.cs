using RimWorld;
using Verse;

namespace Stats.Compat.CE;

public class ColumnWorker_WeaponRanged_Caliber
    : ColumnWorker_Str
{
    protected override string GetValue(ThingRec thing)
    {
        var statReq = StatRequest.For(thing.Def, thing.StuffDef);

        return ColumnDef.stat!.Worker.GetStatDrawEntryLabel(
            ColumnDef.stat!,
            ColumnDef.stat!.Worker.GetValue(statReq),
            ToStringNumberSense.Absolute,
            statReq
        );
    }
    protected override IWidget GetTableCellContent(string? value, ThingRec thing)
    {
        var statReq = StatRequest.For(thing.Def, thing.StuffDef);
        var tooltip = ColumnDef.stat!.Worker.GetExplanationFull(
            statReq,
            ToStringNumberSense.Absolute,
            ColumnDef.stat!.Worker.GetValue(statReq)
        );
        IWidget widget = new Widget_Label(value);
        new WidgetComp_Tooltip(ref widget, tooltip);

        return widget;
    }
}
