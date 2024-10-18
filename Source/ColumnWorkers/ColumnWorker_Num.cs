using RimWorld;
using Verse;

namespace Stats;

public class ColumnWorker_Num : ColumnWorker<ICellWidget<float>>
{
    protected virtual float GetValue(ThingDef thingDef, ThingDef? stuffDef)
    {
        var statReq = StatRequest.For(thingDef, stuffDef);

        if (ColumnDef.stat!.Worker.ShouldShowFor(statReq) == true)
        {
            return ColumnDef.stat!.Worker.GetValue(statReq);
        }

        return 0f;
    }
    protected sealed override ICellWidget<float>? CreateCell(ThingDef thingDef, ThingDef? stuffDef)
    {
        var value = GetValue(thingDef, stuffDef);

        if (value != 0f && float.IsNaN(value) == false)
        {
            var valueStr = ColumnDef.formatString == ""
                ? ColumnDef.stat!.Worker.ValueToString(value, true)
                : value.ToString(ColumnDef.formatString);

            return new CellWidget_Num(value, valueStr);
        }

        return null;
    }
}
