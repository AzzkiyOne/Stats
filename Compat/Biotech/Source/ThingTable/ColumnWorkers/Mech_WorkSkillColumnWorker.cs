using Stats.ThingTable;

namespace Stats.Compat.Biotech.ThingTable;

public sealed class Mech_WorkSkillColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Mech_WorkSkillColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var raceProps = thing.Def.race;

        if (raceProps != null)
        {
            return raceProps.mechFixedSkillLevel;
        }

        return 0m;
    }
}
