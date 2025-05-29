using RimWorld;

namespace Stats.ThingTable;

public sealed class PlantRawNutritionPerDayColumnWorker : NumberColumnWorker<ThingAlike>
{
    public PlantRawNutritionPerDayColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.000/d")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var plantProps = thing.Def.plant;

        if (plantProps != null && plantProps.growDays > 0f)
        {
            var stat = StatDefOf.Nutrition;
            var statRequest = StatRequest.For(thing.Def, null);

            if (stat.Worker.ShouldShowFor(statRequest))
            {
                var nutrition = stat.Worker.GetValue(statRequest);

                if (nutrition > 0f)
                {
                    return (nutrition / plantProps.growDays).ToDecimal(3);
                }
            }
        }

        return 0m;
    }
}
