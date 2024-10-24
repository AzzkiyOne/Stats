using System.Linq;

namespace Stats;

public class ColumnWorker_Stuff_Category : ColumnWorker_Str
{
    protected override string? GetValue(ThingRec thing)
    {
        return thing.Def.stuffProps?.categories.FirstOrDefault().LabelCap;
    }
}
