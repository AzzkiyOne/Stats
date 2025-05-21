namespace Stats.ThingTable;

public sealed class RangedStoppingPowerColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    public RangedStoppingPowerColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        return defaultProj?.stoppingPower.ToString("F1") ?? "";
    }
}
