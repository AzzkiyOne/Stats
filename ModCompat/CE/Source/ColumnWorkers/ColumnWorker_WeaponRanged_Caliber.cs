using RimWorld;
using UnityEngine;
using Verse;

namespace Stats.Compat.CE;

public class ColumnWorker_WeaponRanged_Caliber
    : ColumnWorker_Str
{
    public override string GetValue(ThingRec thing)
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
        var style = new WidgetStyle()
        {
            TextAlign = (TextAnchor)CellStyle,
        };

        return new Widget_Addon_Tooltip(new Widget_Label(value!, style), tooltip);
    }
}
