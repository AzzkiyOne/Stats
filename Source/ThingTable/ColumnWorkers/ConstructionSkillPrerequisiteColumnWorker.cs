namespace Stats.ThingTable;

public sealed class ConstructionSkillPrerequisiteColumnWorker : NumberColumnWorker<ThingAlike>
{
    public ConstructionSkillPrerequisiteColumnWorker(ColumnDef columndef) : base(columndef, false)
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        if (thing.Def.BuildableByPlayer == false)
        {
            return 0m;
        }

        return thing.Def.constructionSkillPrerequisite;
    }
}
