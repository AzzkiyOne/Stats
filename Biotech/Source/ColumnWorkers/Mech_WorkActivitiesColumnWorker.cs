using System.Collections.Generic;
using System.Linq;
using Stats.TableRecords.ThingDef;
using Verse;

namespace Stats.Compat.Biotech;

public sealed class Mech_WorkActivitiesColumnWorker : DefSetColumnWorker<VirtualThing, WorkTypeDef>
{
    public Mech_WorkActivitiesColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override HashSet<WorkTypeDef> GetValue(VirtualThing thing)
    {
        return thing.Def.race?.mechEnabledWorkTypes.ToHashSet() ?? [];
    }
    protected override string GetDefLabel(WorkTypeDef def)
    {
        return def.gerundLabel.CapitalizeFirst();
    }
}
