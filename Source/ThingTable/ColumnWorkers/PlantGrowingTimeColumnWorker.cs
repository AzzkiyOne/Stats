namespace Stats.ThingTable;

public sealed class PlantGrowingTimeColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    public PlantGrowingTimeColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        var plantProps = thing.Def.plant;

        if (plantProps == null)
        {
            return "";
        }

        return $"{plantProps.growDays:0.##} d";
    }
}
