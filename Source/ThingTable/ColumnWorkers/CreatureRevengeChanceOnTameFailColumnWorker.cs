using Verse;

namespace Stats.ThingTable;

public sealed class CreatureRevengeChanceOnTameFailColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    public CreatureRevengeChanceOnTameFailColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        var raceProps = thing.Def.race;

        if (raceProps != null)
        {
            var chance = raceProps.manhunterOnTameFailChance;

            if (chance > 0)
            {
                return chance.ToStringPercent();
            }
        }

        return "";
    }
}
