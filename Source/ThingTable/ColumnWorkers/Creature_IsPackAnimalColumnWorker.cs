namespace Stats.ThingTable;

public sealed class Creature_IsPackAnimalColumnWorker : BooleanColumnWorker<ThingAlike>
{
    public Creature_IsPackAnimalColumnWorker(ColumnDef columndef) : base(columndef, false)
    {
    }
    protected override bool GetValue(ThingAlike thing)
    {
        return thing.Def.race?.packAnimal == true;
    }
}
