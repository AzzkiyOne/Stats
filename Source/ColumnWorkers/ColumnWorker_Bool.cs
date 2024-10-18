using RimWorld;
using Verse;

namespace Stats;

public class ColumnWorker_Bool : ColumnWorker<ICellWidget<bool>>
{
    protected virtual bool GetValue(ThingDef thingDef, ThingDef? stuffDef)
    {
        var statReq = StatRequest.For(thingDef, stuffDef);

        if (ColumnDef.stat!.Worker.ShouldShowFor(statReq) == true)
        {
            return ColumnDef.stat!.Worker.GetValue(statReq) > 0f;
        }

        return false;
    }
    protected sealed override ICellWidget<bool>? CreateCell(ThingDef thingDef, ThingDef? stuffDef)
    {
        var value = GetValue(thingDef, stuffDef);

        if (value)
        {
            return new CellWidget_Bool(value);
        }

        return null;
    }
}
