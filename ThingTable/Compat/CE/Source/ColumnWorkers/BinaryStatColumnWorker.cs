using System.Linq;
using CombatExtended;

namespace Stats.ThingTable.Compat.CE;

public class BinaryStatColumnWorker : StatColumnWorker
{
    protected char Separator { get; }
    protected BinaryStatColumnWorker(ColumnDef columnDef, char separator)
        : base(
            columnDef.stat == CE_StatDefOf.ReloadTime
                ? CE_StatDefOf.MagazineCapacity
                : columnDef.stat!,
            columnDef.statValueExplanationType
        )
    {
        Separator = separator;
    }
    public static StatColumnWorker MakeLeft(ColumnDef columnDef) => Make(columnDef, true);
    public static StatColumnWorker MakeRight(ColumnDef columnDef) => Make(columnDef, false);
    private static StatColumnWorker Make(ColumnDef columnDef, bool makeLeft)
    {
        var stat = columnDef.stat!;
        var separator = ' ';

        if (stat == StatDefOf.MeleeDamage)
        {
            separator = '-';
        }
        else if (stat == StatDefOf.MeleeWeapon_AverageArmorPenetration)
        {
            separator = ',';
        }
        else if (stat == CE_StatDefOf.MagazineCapacity || stat == CE_StatDefOf.ReloadTime)
        {
            separator = '/';
        }
        else if (stat == RimWorld.StatDefOf.ArmorRating_Sharp || stat == RimWorld.StatDefOf.ArmorRating_Blunt)
        {
            separator = '~';
        }

        if (separator != ' ')
        {
            if (makeLeft)
            {
                return new BinaryStatColumnWorkerLeft(columnDef, separator);
            }

            return new BinaryStatColumnWorkerRight(columnDef, separator);
        }

        return Make(columnDef);
    }
}

file sealed class BinaryStatColumnWorkerLeft : BinaryStatColumnWorker
{
    public BinaryStatColumnWorkerLeft(ColumnDef columnDef, char separator) : base(columnDef, separator)
    {
    }
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        return base.GetStatDrawEntryLabel(thing).Split(Separator).First();
    }
}

file sealed class BinaryStatColumnWorkerRight : BinaryStatColumnWorker
{
    public BinaryStatColumnWorkerRight(ColumnDef columnDef, char separator) : base(columnDef, separator)
    {
    }
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        return base.GetStatDrawEntryLabel(thing).Split(Separator).Last();
    }
}
