using System.Linq;

namespace Stats;

public class ColumnWorker_Stuff_Category : ColumnWorker<ICellWidget<string>>
{
    protected override ICellWidget<string>? CreateCell(ThingRec thing)
    {
        var value = thing.Def.stuffProps?.categories.FirstOrDefault();

        if (value != null)
        {
            return new CellWidget_Str(value.LabelCap);
        }

        return null;
    }
}
