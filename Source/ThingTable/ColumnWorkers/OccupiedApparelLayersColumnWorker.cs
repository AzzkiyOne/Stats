using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Stats.ThingTable;

public sealed class OccupiedApparelLayersColumnWorker : DefSetColumnWorker<ThingAlike, ApparelLayerDef>
{
    public OccupiedApparelLayersColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override HashSet<ApparelLayerDef> GetValue(ThingAlike thing)
    {
        return thing.Def.apparel?.layers.ToHashSet() ?? [];
    }
}
