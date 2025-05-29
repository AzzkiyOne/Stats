using Verse;

namespace Stats.ThingTable;

public sealed class PlantFertilitySensitivityColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    public PlantFertilitySensitivityColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        var plantProps = thing.Def.plant;

        if (plantProps == null || plantProps.fertilitySensitivity == 0f)
        {
            return "";
        }

        return plantProps.fertilitySensitivity.ToStringPercent();
    }
}
