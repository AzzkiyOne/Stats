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
    public override GenTable.Cell? GetCell(ThingAlike thing)
    {
        return new GenTable.Cell_DefRef(Column, thing.Def, thing.Stuff);
    }
}

public class ColumnWorker_Stat : ColumnWorker
{
    protected virtual StatDef Stat => Column.Stat;
    public override GenTable.Cell? GetCell(ThingAlike thing)
    {
        var statReq = StatRequest.For(thing.Def, thing.Stuff);

        if (Stat.Worker.ShouldShowFor(statReq) == false)
        {
            return null;
        }

        var statValue_Num = Stat.Worker.GetValue(statReq);
        string? statValue_Str = null;

        if (Column.FormatValue)
        {
            statValue_Str = Stat.Worker.GetStatDrawEntryLabel(
                Stat,
                statValue_Num,
                ToStringNumberSense.Absolute,
                // This is necessary (despite statReq being "optional") because some mods
                // override this signature.
                StatRequest.For(thing.Def, thing.Stuff)
            );
        }

        if (Column.Type == GenTable.ColumnType.Boolean)
        {
            return new GenTable.Cell_Bool(Column, statValue_Num);
        }

        return new GenTable.Cell_Num(Column, statValue_Num, statValue_Str);
    }
}

public class ColumnWorker_WeaponRange : ColumnWorker
{
    public override GenTable.Cell? GetCell(ThingAlike thing)
    {
        if (thing.Def.IsRangedWeapon && thing.Def.Verbs.Count > 0)
        {
            var range = thing.Def.Verbs.First(v => v.isPrimary)?.range;

            if (range is float _range)
            {
                return new GenTable.Cell_Num(Column, _range);
            }
        }

        return null;
    }
}