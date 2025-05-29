namespace Stats.ThingTable;

public sealed class PlantMinGrowingSkillToSowColumnWorker : NumberColumnWorker<ThingAlike>
{
    public PlantMinGrowingSkillToSowColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        return thing.Def.plant?.sowMinSkill ?? 0m;
    }
}
