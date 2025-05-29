using RimWorld;

namespace Stats.ThingTable;

public sealed class PlantNutritionPerHarvestPerDayColumnWorker : NumberColumnWorker<ThingAlike>
{
    public PlantNutritionPerHarvestPerDayColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.000/d")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        // TODO: This is mostly copy paste from PlantNutritionPerHarvestColumnWorker.
        var plantProps = thing.Def.plant;

        if (plantProps?.harvestedThingDef != null)
        {
            var statRequest = StatRequest.For(plantProps.harvestedThingDef, null);

            if (StatDefOf.Nutrition.Worker.ShouldShowFor(statRequest))
            {
                var result = plantProps.harvestYield * StatDefOf.Nutrition.Worker.GetValue(statRequest);

                return (result / plantProps.growDays).ToDecimal(3);
            }
        }

        return 0m;
    }
}
