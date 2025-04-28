using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Widgets;
using Stats.Widgets.FilterWidgets;
using Verse;

namespace Stats.ColumnWorkers;

public sealed class StuffCategoryColumnWorker : ColumnWorker
{
    public override TableColumnCellStyle CellStyle => TableColumnCellStyle.String;
    // TODO: "categorieS" is for a reason.
    private static readonly Func<ThingAlike, StuffCategoryDef?> GetStuffCatDef = FunctionExtensions.Memoized(
        (ThingAlike thing) => thing.Def.stuffProps?.categories.FirstOrDefault()
    );
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        var stuffCatDef = GetStuffCatDef(thing);

        if (stuffCatDef == null)
        {
            return null;
        }

        return new Label(stuffCatDef.LabelCap);
    }
    public override FilterWidget GetFilterWidget()
    {
        return new OneToManyOptionsFilter<StuffCategoryDef?>(
            GetStuffCatDef,
            DefDatabase<StuffCategoryDef>.AllDefsListForReading.OrderBy(stuffCatDef => stuffCatDef.label),
            stuffCatDef => new Label(stuffCatDef.LabelCap)
        );
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        var label1 = GetStuffCatDef(thing1)?.label;
        var label2 = GetStuffCatDef(thing2)?.label;

        return Comparer<string?>.Default.Compare(label1, label2);
    }
}
