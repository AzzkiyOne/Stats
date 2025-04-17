using System.Linq;

namespace Stats;

public class ColumnWorker_Apparel_Layers
    : ColumnWorker_Str
{
    protected override string? GetValue(ThingRec thing)
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
