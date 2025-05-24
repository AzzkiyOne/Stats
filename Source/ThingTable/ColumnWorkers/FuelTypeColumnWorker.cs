using System;
using System.Collections.Generic;
using System.Linq;
using Stats.Widgets;
using Verse;

namespace Stats.ThingTable;

public sealed class FuelTypeColumnWorker : ColumnWorker<ThingAlike>
{
    public FuelTypeColumnWorker(ColumnDef columnDef) : base(columnDef, ColumnCellStyle.String)
    {
    }
    private static readonly Func<ThingAlike, ThingDef?> GetFuelTypeDef =
    FunctionExtensions.Memoized((ThingAlike thing) =>
    {
        return thing.Def.GetRefuelableCompProperties()?.fuelFilter?.AnyAllowedDef;
    });
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        var fuelTypeDef = GetFuelTypeDef(thing);

        if (fuelTypeDef == null)
        {
            return null;
        }

        void openDefInfoDialog()
        {
            Draw.DefInfoDialog(fuelTypeDef);
        }

        return new HorizontalContainer(
            [
                new ThingIcon(fuelTypeDef).ToButtonGhostly(openDefInfoDialog),
                new Label(fuelTypeDef.LabelCap),
            ],
            Globals.GUI.Pad
        );
    }
    public override FilterWidget<ThingAlike> GetFilterWidget(IEnumerable<ThingAlike> tableRecords)
    {
        var fuelTypeDefs = tableRecords
            .Select(GetFuelTypeDef)
            .Distinct()
            .OrderBy(fuelTypeDef => fuelTypeDef?.label);

        return new OneToManyFilter<ThingAlike, ThingDef?>(
            GetFuelTypeDef,
            fuelTypeDefs,
            fuelTypeDef => fuelTypeDef == null
                ? new Label("")
                : new HorizontalContainer(
                    [
                        new ThingIcon(fuelTypeDef),
                        new Label(fuelTypeDef.LabelCap),
                    ],
                    Globals.GUI.Pad
                )
        );
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        var fuelTypeDefLabel1 = GetFuelTypeDef(thing1)?.label;
        var fuelTypeDefLabel2 = GetFuelTypeDef(thing2)?.label;

        return Comparer<string?>.Default.Compare(fuelTypeDefLabel1, fuelTypeDefLabel2);
    }
}
