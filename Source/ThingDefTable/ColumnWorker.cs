using System.Linq;
using RimWorld;
using Verse;

namespace Stats.ThingDefTable;

public abstract class ColumnWorker : GenTable.ColumnWorker<ThingAlike>
{
    public ColumnDef Column { get; set; }
}

public class ColumnWorker_Label : ColumnWorker
{
    public override GenTable.ICell? GetCell(ThingAlike thing)
    {
        return new GenTable.Cell_DefRef(thing.Def, thing.Stuff);
    }
}

public class ColumnWorker_Stat : ColumnWorker
{
    protected virtual StatDef Stat => Column.Stat;
    public override GenTable.ICell? GetCell(ThingAlike thing)
    {
        var statReq = StatRequest.For(thing.Def, thing.Stuff);

        if (Stat.Worker.ShouldShowFor(statReq) == false)
        {
            return null;
        }

        var statValue_num = Stat.Worker.GetValue(statReq);
        string? statValue_str = null;

        if (Column.FormatValue)
        {
            statValue_str = Stat.Worker.GetStatDrawEntryLabel(
                Stat,
                statValue_num,
                ToStringNumberSense.Absolute,
                // This is necessary (despite statReq being "optional") because some mods
                // override this signature.
                StatRequest.For(thing.Def, thing.Stuff)
            );
        }

        if (Column.Type == GenTable.ColumnType.Boolean)
        {
            return new GenTable.Cell_Bool(statValue_num);
        }

        return new GenTable.Cell_Num(statValue_num, statValue_str);
    }
}

public class ColumnWorker_WeaponRange : ColumnWorker
{
    public override GenTable.ICell? GetCell(ThingAlike thing)
    {
        if (thing.Def.IsRangedWeapon && thing.Def.Verbs.Count > 0)
        {
            var range = thing.Def.Verbs.First(v => v.isPrimary)?.range;

            if (range is float _range)
            {
                return new GenTable.Cell_Num(_range);
            }
        }

        return null;
    }
}