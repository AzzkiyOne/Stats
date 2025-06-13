using RimWorld;

namespace Stats.ThingTable;

public sealed class Creature_CaravanCarryingCapacityColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Creature_CaravanCarryingCapacityColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0 kg")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var raceProps = thing.Def.race;

        if (raceProps != null)
        {
            return (raceProps.baseBodySize * MassUtility.MassCapacityPerBodySize).ToDecimal(0);
        }

        return 0m;
    }
}
