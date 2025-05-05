namespace Stats.ThingTable;

public static class RangedMissRadiusColumnWorker
{
    public static NumberColumnWorker<ThingAlike> Make(ColumnDef _) => new(GetValue);
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
