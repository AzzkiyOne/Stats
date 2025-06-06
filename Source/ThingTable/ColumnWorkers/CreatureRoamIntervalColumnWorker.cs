namespace Stats.ThingTable;

public sealed class CreatureRoamIntervalColumnWorker : NumberColumnWorker<ThingAlike>
{
    public CreatureRoamIntervalColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.0 d")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var raceProps = thing.Def.race;

        if (raceProps?.roamMtbDays is float roamInterval && roamInterval > 0f)
        {
            // Realistically roamMtbDays is an int, but since
            // it is float, we use precision of 1 just in case.
            return roamInterval.ToDecimal(1);
        }

        return 0m;
    }
}
