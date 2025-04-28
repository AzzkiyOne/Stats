using System;
using Stats.Widgets;
using Stats.Widgets.FilterWidgets;
using Verse;

namespace Stats.ColumnWorkers.RangedWeapon;

public sealed class RPMColumnWorker : ColumnWorker
{
    public override TableColumnCellStyle CellStyle => TableColumnCellStyle.Number;
    private static readonly Func<ThingAlike, float> GetValue = FunctionExtensions.Memoized(
        (ThingAlike thing) =>
        {
            var verb = thing.Def.Verbs.Primary();

            if (verb is { Ranged: true, showBurstShotStats: true, burstShotCount: > 1 })
            {
                return 60f / verb.ticksBetweenBurstShots.TicksToSeconds();
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

        return new Label(value.ToString("0 rpm"));
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
