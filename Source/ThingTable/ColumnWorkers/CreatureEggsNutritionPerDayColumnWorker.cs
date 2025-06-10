using RimWorld;

namespace Stats.ThingTable;

public sealed class CreatureEggsNutritionPerDayColumnWorker : NumberColumnWorker<ThingAlike>
{
    public CreatureEggsNutritionPerDayColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.00/d")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var eggLayerCompProps = thing.Def.GetCompProperties<CompProperties_EggLayer>();

        if (eggLayerCompProps is { eggLayIntervalDays: > 0 })
        {
            var eggType = eggLayerCompProps.eggUnfertilizedDef ?? eggLayerCompProps.eggFertilizedDef;
            var statRequest = StatRequest.For(eggType, null);
            var nutritionStatWorker = StatDefOf.Nutrition.Worker;

            if (nutritionStatWorker.ShouldShowFor(statRequest))
            {
                var eggNutrition = nutritionStatWorker.GetValue(statRequest);
                var eggsPerDay = eggLayerCompProps.eggCountRange.Average / eggLayerCompProps.eggLayIntervalDays;

                return (eggsPerDay * eggNutrition).ToDecimal(2);
            }
        }

        return 0m;
    }
}
