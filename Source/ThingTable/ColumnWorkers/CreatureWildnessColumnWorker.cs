using Verse;

namespace Stats.ThingTable;

public sealed class CreatureWildnessColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    public CreatureWildnessColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        var raceProps = thing.Def.race;

        if (raceProps != null)
        {
            var wildness = raceProps.wildness;

            if (wildness > 0f)
            {
                return wildness.ToStringPercent();
            }
        }

        return "";
    }
}
