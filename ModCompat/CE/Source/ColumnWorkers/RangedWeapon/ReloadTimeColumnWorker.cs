using System;
using CombatExtended;
using RimWorld;
using Stats.ColumnWorkers;
using Stats.Widgets;
using Stats.Widgets.FilterWidgets;

namespace Stats.ModCompat.CE.ColumnWorkers.RangedWeapon;

public sealed class ReloadTimeColumnWorker : ColumnWorker
{
    public override TableColumnCellStyle CellStyle => TableColumnCellStyle.Number;
    private static readonly Func<ThingAlike, float> GetValue = FunctionExtensions.Memoized(
        (ThingAlike thing) =>
        {
            var statReq = StatRequest.For(thing.Def, thing.StuffDef);

            if (CE_StatDefOf.MagazineCapacity.Worker.ShouldShowFor(statReq))
            {
                return CE_StatDefOf.ReloadTime.Worker.GetValue(statReq);
            }

            return default;
        }
    );
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        var value = GetValue(thing);

        if (value == default)
        {
            return null;
        }

        return new Label(value.ToString("0.00 s"));
    }
    public override FilterWidget GetFilterWidget()
    {
        return new NumberFilter<float>(GetValue);
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return GetValue(thing1).CompareTo(GetValue(thing2));
    }
}
