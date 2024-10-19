using RimWorld;
using Verse;

namespace Stats;

public class ColumnWorker_Str : ColumnWorker<ICellWidget<string>>
{
    protected virtual string? GetValue(ThingRec thing)
    {
        var statReq = StatRequest.For(thing.Def, thing.StuffDef);

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
    protected sealed override ICellWidget<string>? CreateCell(ThingRec thing)
    {
        var value = GetValue(thing);

        if (value?.Length > 0)
        {
            return new CellWidget_Str(value);
        }

        return null;
    }
}
