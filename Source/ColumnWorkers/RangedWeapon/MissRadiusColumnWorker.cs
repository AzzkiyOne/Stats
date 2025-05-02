namespace Stats.ColumnWorkers.RangedWeapon;

public sealed class MissRadiusColumnWorker : NumberColumnWorker
{
    public MissRadiusColumnWorker() : base(GetValue)
    {
    }
    private static decimal GetValue(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();

        if (verb?.ForcedMissRadius == null)
        {
            return 0m;
        }

        return verb.ForcedMissRadius.ToDecimal("F1");
    }
}
