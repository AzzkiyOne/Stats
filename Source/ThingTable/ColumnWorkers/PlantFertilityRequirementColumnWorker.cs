using Verse;

namespace Stats.ThingTable;

public sealed class PlantFertilityRequirementColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    public PlantFertilityRequirementColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        var plantProps = thing.Def.plant;

        if (plantProps == null || plantProps.fertilityMin == 0f)
        {
            return "";
        }

        return plantProps.fertilityMin.ToStringPercent();
    }
}
