using RimWorld;

namespace Stats.ThingTable;

public sealed class CreatureGestationTimeColumnWorker : NumberColumnWorker<ThingAlike>
{
    public CreatureGestationTimeColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.00 d")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var raceProps = thing.Def.race;

        if (raceProps != null)
        {
            var gestationTime = AnimalProductionUtility.GestationDaysLitter(thing.Def);

            if (gestationTime > 0f)
            {
                return gestationTime.ToDecimal(2);
            }
        }

        return 0m;
    }
}
