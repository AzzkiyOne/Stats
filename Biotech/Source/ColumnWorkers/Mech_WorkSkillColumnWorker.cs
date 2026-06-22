using Stats.TableRecords.ThingDef;
using Stats.ObjectTable.ColumnWorkers;

namespace Stats.Compat.Biotech;

public sealed class Mech_WorkSkillColumnWorker : NumberColumnWorker<VirtualThing>
{
    public Mech_WorkSkillColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override decimal GetCellValueSource(VirtualThing thing)
    {
        var raceProps = thing.Def.race;

        if (raceProps != null)
        {
            return raceProps.mechFixedSkillLevel;
        }

        return 0m;
    }
}
