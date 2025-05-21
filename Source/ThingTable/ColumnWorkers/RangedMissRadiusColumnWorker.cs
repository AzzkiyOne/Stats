namespace Stats.ThingTable;

public sealed class RangedMissRadiusColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    public RangedMissRadiusColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();

        if (verb?.ForcedMissRadius > 0f)
        {
            return verb.ForcedMissRadius.ToString("0.#");
        }

        return "";
    }
}
