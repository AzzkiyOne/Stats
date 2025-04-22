using System.Linq;
using Stats.ColumnWorkers.Generic;

namespace Stats.ColumnWorkers;

public sealed class StuffCategoryColumnWorker
    : StringColumnWorker
{
    protected override string? GetValue(ThingAlike thing)
    {
        return thing.Def.stuffProps?.categories.FirstOrDefault().LabelCap;
    }
}
