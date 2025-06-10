using RimWorld;

namespace Stats.ThingTable;

public sealed class CreatureMilkNutritionPerDayColumnWorker : NumberColumnWorker<ThingAlike>
{
    public CreatureMilkNutritionPerDayColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.00/d")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var milkableCompProps = thing.Def.GetCompProperties<CompProperties_Milkable>();

        if (milkableCompProps is { milkDef: not null, milkAmount: > 0, milkIntervalDays: > 0 })
        {
            var statRequest = StatRequest.For(milkableCompProps.milkDef, null);
            var nutritionStatWorker = StatDefOf.Nutrition.Worker;

            if (nutritionStatWorker.ShouldShowFor(statRequest))
            {
                var milkNutrition = nutritionStatWorker.GetValue(statRequest);
                var milkPerDay = (float)milkableCompProps.milkAmount / milkableCompProps.milkIntervalDays;

                return (milkPerDay * milkNutrition).ToDecimal(2);
            }
        }

        return 0m;
    }
}
