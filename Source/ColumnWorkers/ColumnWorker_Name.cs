using Verse;

namespace Stats;

public class ColumnWorker_Name : ColumnWorker<ICellWidget<ThingAlike>>
{
    protected override ICellWidget<ThingAlike>? CreateCell(ThingDef thingDef, ThingDef? stuffDef)
    {
        return new CellWidget_Thing(new ThingAlike(thingDef, stuffDef));
    }
}
