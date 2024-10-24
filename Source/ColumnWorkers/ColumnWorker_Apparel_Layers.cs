using System.Linq;
using Verse;

namespace Stats;

public class ColumnWorker_Apparel_Layers : ColumnWorker_Str
{
    protected override string? GetValue(ThingRec thing)
    {
        return string.Join(
            ", ",
            thing.Def.apparel?.layers.Select(layer => layer.LabelCap)
        );
    }
}
