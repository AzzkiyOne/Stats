using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Widgets;
using Verse;

namespace Stats.ThingTable;

public sealed class ResourcesRequiredForConstructionColumnWorker : ColumnWorker<ThingAlike>
{
    public ResourcesRequiredForConstructionColumnWorker(ColumnDef columnDef) : base(columnDef, ColumnCellStyle.Number)
    {
    }
    private static readonly Func<ThingAlike, Dictionary<ThingDef, int>?> GetCostList =
    FunctionExtensions.Memoized((ThingAlike thing) =>
    {
        if (thing.Def.BuildableByPlayer == false)
        {
            return null;
        }

        var thingDefCostList = thing.Def.CostList;
        var costList = new Dictionary<ThingDef, int>();

        if (thingDefCostList?.Count > 0)
        {
            foreach (var thingDefCount in thingDefCostList)
            {
                costList[thingDefCount.thingDef] = thingDefCount.count;
            }
        }

        if (thing.StuffDef != null && thing.Def.CostStuffCount > 0)
        {
            var costListEntryExists = costList.ContainsKey(thing.StuffDef);
            var stuffCost = thing.StuffDef.smallVolume
                ? thing.Def.CostStuffCount * ThingDef.SmallUnitPerVolume
                : thing.Def.CostStuffCount;

            if (costListEntryExists)
            {
                costList[thing.StuffDef] += stuffCost;
            }
            else
            {
                costList.Add(thing.StuffDef, stuffCost);
            }
        }

        return costList;
    });
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        var costList = GetCostList(thing);

        if (costList == null || costList.Count == 0)
        {
            return null;
        }

        var rows = new List<Widget>(costList.Keys.Count);

        foreach (var thingDef in costList.Keys.OrderBy(thingDef => thingDef.label))
        {
            void openDefInfoDialog()
            {
                Draw.DefInfoDialog(thingDef);
            }

            var cost = costList[thingDef];
            var row = new HorizontalContainer(
                [
                    new Label(cost.ToString()).PaddingRel(1f, 0f, 0f, 0f),
                    new ThingIcon(thingDef).ToButtonGhostly(openDefInfoDialog, thingDef.LabelCap),
                ],
                Globals.GUI.PadSm,
                true
            ).WidthRel(1f);

            rows.Add(row);
        }

        return new VerticalContainer(rows);
    }
    private static readonly Func<ThingAlike, HashSet<ThingDef>> GetResourceDefs =
    FunctionExtensions.Memoized((ThingAlike thing) => GetCostList(thing)?.Keys.ToHashSet() ?? []);
    public override FilterWidget<ThingAlike> GetFilterWidget(IEnumerable<ThingAlike> tableRecords)
    {
        return Make.MTMThingDefFilter(GetResourceDefs, tableRecords);
    }
    private static readonly Func<ThingAlike, int> GetTotalResourcesCost =
    FunctionExtensions.Memoized((ThingAlike thing) =>
    {
        var costList = GetCostList(thing);

        if (costList == null)
        {
            return 0;
        }

        var result = 0;

        foreach (var (thingDef, count) in costList)
        {
            var statRequest = StatRequest.For(thingDef, null);

            if (StatDefOf.MarketValue.Worker.ShouldShowFor(statRequest))
            {
                // TODO: Cache Worker.GetValue result.
                result += (int)StatDefOf.MarketValue.Worker.GetValue(statRequest) * count;
            }
        }

        return result;
    });
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        // Why not to use just market value?
        // - Not every thing has market value.
        // - Market values isn't a very precise measurement of how expensive it is to construct something.
        var totalResourcesCost1 = GetTotalResourcesCost(thing1);
        var totalResourcesCost2 = GetTotalResourcesCost(thing2);

        return totalResourcesCost1.CompareTo(totalResourcesCost2);
    }
}
