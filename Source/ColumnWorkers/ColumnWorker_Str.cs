using RimWorld;
using Verse;

namespace Stats;

public class ColumnWorker_Str : ColumnWorker<ICellWidget<string>>
{
    protected virtual string? GetValue(ThingDef thingDef, ThingDef? stuffDef)
    {
        var statReq = StatRequest.For(thingDef, stuffDef);

        if (ColumnDef.stat!.Worker.ShouldShowFor(statReq) == true)
        {
            return ColumnDef.stat!.Worker.GetStatDrawEntryLabel(
                ColumnDef.stat!,
                ColumnDef.stat!.Worker.GetValue(statReq),
                ToStringNumberSense.Absolute,
                statReq
            );
        }

        return null;
    }
    protected sealed override ICellWidget<string>? CreateCell(ThingDef thingDef, ThingDef? stuffDef)
    {
        var value = GetValue(thingDef, stuffDef);

        if (value?.Length > 0)
        {
            return new CellWidget_Str(value);
        }

        return null;
    }
}
