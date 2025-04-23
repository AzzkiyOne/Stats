using System.Linq;
using Stats.ColumnWorkers.Generic;

namespace Stats.ColumnWorkers.Apparel;

public sealed class LayersColumnWorker : StringColumnWorker
{
    protected override string? GetValue(ThingAlike thing)
    {
        var labels =
            thing
            .Def
            .apparel?
            .layers
            .OrderBy(def => def.label)
            .Select(layer => layer.LabelCap);

        return string.Join("\n", labels);
    }
}
