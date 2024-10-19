namespace Stats;

public class ColumnWorker_Name : ColumnWorker<ICellWidget<ThingAlike>>
{
    protected override ICellWidget<ThingAlike>? CreateCell(ThingRec thing)
    {
        return new CellWidget_Thing(new ThingAlike(thing.Def, thing.StuffDef));
    }
}
