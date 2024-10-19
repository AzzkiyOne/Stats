using RimWorld;

namespace Stats;

public class ColumnWorker_Num : ColumnWorker<ICellWidget<float>>
{
    protected virtual float GetValue(ThingRec thing)
    {
        var statReq = StatRequest.For(thing.Def, thing.StuffDef);

        if (ColumnDef.stat!.Worker.ShouldShowFor(statReq) == true)
        {
            return ColumnDef.stat!.Worker.GetValue(statReq);
        }

        return 0f;
    }
    protected sealed override ICellWidget<float>? CreateCell(ThingRec thing)
    {
        var value = GetValue(thing);

        if (value != 0f && float.IsNaN(value) == false)
        {
            var valueStr = ColumnDef.formatString == ""
                ? ColumnDef.stat!.Worker.ValueToString(value, true)
                : value.ToString(ColumnDef.formatString);

            return new CellWidget_Num(value, valueStr);
        }

        return null;
    }
    public override IFilterWidget GetFilterWidget()
    {
        return new FilterWidget_Num(thing => GetCell(thing)?.Value);
    }
}
