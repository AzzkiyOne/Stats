namespace Stats.ThingTable;

public sealed class CreatureIsPackAnimalColumnWorker : BooleanColumnWorker<ThingAlike>
{
    public CreatureIsPackAnimalColumnWorker(ColumnDef columndef) : base(columndef, false)
    {
    }
    protected override bool GetValue(ThingAlike thing)
    {
        return thing.Def.race?.packAnimal == true;
    }
}
