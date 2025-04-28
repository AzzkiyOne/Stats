using System;
using Stats.Widgets;
using Stats.Widgets.FilterWidgets;

namespace Stats.ColumnWorkers.RangedWeapon;

public sealed class StoppingPowerColumnWorker : ColumnWorker
{
    public override TableColumnCellStyle CellStyle => TableColumnCellStyle.Number;
    private static readonly Func<ThingAlike, float> GetValue = FunctionExtensions.Memoized(
        (ThingAlike thing) =>
        {
            var verb = thing.Def.Verbs.Primary();
            var defaultProj = verb?.defaultProjectile?.projectile;

            return defaultProj?.stoppingPower ?? default;
        }
    );
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        var value = GetValue(thing);

        if (value == default)
        {
            return null;
        }

        return new Label(value.ToString("F1"));
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
