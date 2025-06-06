using RimWorld;
using Verse;

namespace Stats.ThingTable;

public sealed class CreatureRevengeChanceOnHarmColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    public CreatureRevengeChanceOnHarmColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        var raceProps = thing.Def.race;

        if (raceProps != null)
        {
            var chance = PawnUtility.GetManhunterOnDamageChance(raceProps);

            if (chance > 0f)
            {
                return chance.ToStringPercent();

            }
        }

        return "";
    }
}
