using System.Collections.Generic;
using System.Linq;
using Stats.ThingTable;
using Verse;

namespace Stats.Compat.Biotech.ThingTable;

public sealed class Mech_WorkActivitiesColumnWorker : DefSetColumnWorker<ThingAlike, WorkTypeDef>
{
    public Mech_WorkActivitiesColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override HashSet<WorkTypeDef> GetValue(ThingAlike thing)
    {
        return thing.Def.race?.mechEnabledWorkTypes.ToHashSet() ?? [];
    }
    protected override string GetDefLabel(WorkTypeDef def)
    {
        return def.gerundLabel.CapitalizeFirst();
    }
}
