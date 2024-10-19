using RimWorld;

namespace Stats;

public class ColumnWorker_Bool : ColumnWorker<ICellWidget<bool>>
{
    protected virtual bool GetValue(ThingRec thing)
    {
        var statReq = StatRequest.For(thing.Def, thing.StuffDef);

        if (ColumnDef.stat!.Worker.ShouldShowFor(statReq) == true)
        {
            return ColumnDef.stat!.Worker.GetValue(statReq) > 0f;
        }

        return false;
    }
    protected sealed override ICellWidget<bool>? CreateCell(ThingRec thing)
    {
        var value = GetValue(thing);

        if (value)
        {
            return new CellWidget_Bool(value);
        }

        return null;
    }

    public override IFilterWidget GetFilterWidget()
    {
        return new FilterWidget_Bool(thing => GetCell(thing)?.Value);
    }
}
