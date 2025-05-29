using UnityEngine;

namespace Stats.ThingTable;

public sealed class PlantHarvestYieldColumnWorker : NumberColumnWorker<ThingAlike>
{
    public PlantHarvestYieldColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var plantProps = thing.Def.plant;

        if (plantProps == null)
        {
            return 0m;
        }

        return Mathf.CeilToInt(plantProps.harvestYield);
    }
}
