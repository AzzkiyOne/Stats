namespace Stats.ThingTable;

public sealed class CreatureNuzzleIntervalColumnWorker : NumberColumnWorker<ThingAlike>
{
    public CreatureNuzzleIntervalColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.0 h")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var raceProps = thing.Def.race;

        if (raceProps != null)
        {
            var interval = raceProps.nuzzleMtbHours;

            if (interval > 0f)
            {
                return interval.ToDecimal(1);

            }
        }

        return 0m;
    }
}
