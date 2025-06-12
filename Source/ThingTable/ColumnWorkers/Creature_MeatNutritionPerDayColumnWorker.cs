using RimWorld;

namespace Stats.ThingTable;

public sealed class Creature_MeatNutritionPerDayColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Creature_MeatNutritionPerDayColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.00/d")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var raceProps = thing.Def.race;

        if (raceProps is { meatDef: not null })
        {
            var statRequest = StatRequest.For(thing.Def.race.meatDef, null);
            var nutritionStatWorker = StatDefOf.Nutrition.Worker;

            if (nutritionStatWorker.ShouldShowFor(statRequest))
            {
                var growthTime = AnimalProductionUtility.DaysToAdulthood(thing.Def);

                if (growthTime > 0f)
                {
                    var meatNutrition = nutritionStatWorker.GetValue(statRequest);
                    var meatAmount = AnimalProductionUtility.AdultMeatAmount(thing.Def);
                    var meatPerDay = meatAmount / growthTime;

                    return (meatPerDay * meatNutrition).ToDecimal(2);
                }
            }
        }

        return 0m;
    }
}
