using System;
using System.Collections;
using System.Collections.Generic;
using RimWorld;
using Stats.Widgets;

namespace Stats.ThingTable;

public sealed class BuildingRecreationTypeColumnWorker : ColumnWorker<ThingAlike>
{
    public BuildingRecreationTypeColumnWorker(ColumnDef columnDef) : base(columnDef, ColumnCellStyle.String)
    {
    }
    private static readonly Func<ThingAlike, JoyKindDef?> GetJoyKindDef =
    FunctionExtensions.Memoized((ThingAlike thing) =>
    {
        return thing.Def.building?.joyKind;
    });
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        var joyKindDef = GetJoyKindDef(thing);

        if (joyKindDef == null)
        {
            return null;
        }

        return new Label(joyKindDef.LabelCap);
    }
    public override FilterWidget<ThingAlike> GetFilterWidget(IEnumerable<ThingAlike> tableRecords)
    {
        return Make.OTMDefFilter(GetJoyKindDef, tableRecords);
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        var joyKindDefLabel1 = GetJoyKindDef(thing1)?.label;
        var joyKindDefLabel2 = GetJoyKindDef(thing2)?.label;

        return Comparer.Default.Compare(joyKindDefLabel1, joyKindDefLabel2);
    }
}
