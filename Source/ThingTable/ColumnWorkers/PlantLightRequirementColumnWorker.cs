using Verse;

namespace Stats.ThingTable;

public sealed class PlantLightRequirementColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    public PlantLightRequirementColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        var plantProps = thing.Def.plant;

        if (plantProps == null || plantProps.growMinGlow == 0f)
        {
            return "";
        }

        return plantProps.growMinGlow.ToStringPercent();
    }
}
