using RimWorld;
using Verse;

namespace Stats.Compat.CE;

public class ColumnWorker_WeaponRanged_Caliber : ColumnWorker<ICellWidget<string>>
{
    protected override ICellWidget<string>? CreateCell(ThingRec thing)
    {
        var statReq = StatRequest.For(thing.Def, thing.StuffDef);

        if (ColumnDef.stat!.Worker.ShouldShowFor(statReq) == true)
        {
            var value = ColumnDef.stat!.Worker.GetValue(statReq);
            var valueStr = ColumnDef.stat!.Worker.GetStatDrawEntryLabel(
                ColumnDef.stat!,
                ColumnDef.stat!.Worker.GetValue(statReq),
                ToStringNumberSense.Absolute,
                statReq
            );
            var valueExpl = ColumnDef.stat!.Worker.GetExplanationFull(statReq, ToStringNumberSense.Absolute, value);

            return new CellWidget_Str(valueStr, valueExpl);
        }

        return null;
    }
}
