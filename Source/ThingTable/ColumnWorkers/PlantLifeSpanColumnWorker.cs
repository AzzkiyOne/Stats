namespace Stats.ThingTable;

public sealed class PlantLifeSpanColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    public PlantLifeSpanColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        var plantProps = thing.Def.plant;

        if (plantProps == null)
        {
            return "";
        }

        return $"{plantProps.LifespanDays:0.##} d";
    }
}
