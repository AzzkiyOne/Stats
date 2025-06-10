using RimWorld;

namespace Stats.ThingTable;

public sealed class CreatureLeatherPerDayColumnWorker : NumberColumnWorker<ThingAlike>
{
    public CreatureLeatherPerDayColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.0/d")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var raceProps = thing.Def.race;

        if (raceProps != null)
        {
            var statRequest = StatRequest.For(thing.Def, null);
            var leatherAmountStatWorker = StatDefOf.LeatherAmount.Worker;

            if (leatherAmountStatWorker.ShouldShowFor(statRequest))
            {
                var growthTime = AnimalProductionUtility.DaysToAdulthood(thing.Def);

                if (growthTime > 0f)
                {
                    var leatherAmount = leatherAmountStatWorker.GetValue(statRequest);

                    return (leatherAmount / growthTime).ToDecimal(1);
                }
            }
        }

        return 0m;
    }
}
