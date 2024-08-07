using System.Collections.Generic;
using Verse;

namespace Stats;

static class ColumnSetDB
{
    static private readonly Dictionary<string, ColumnSet> columnSets = new() { ["Root"] = new ColumnSet() };
    static public ColumnSet? GetColumnSetForCatDef(ThingCategoryDef catDef)
    {
        columnSets.TryGetValue(catDef.defName, out ColumnSet? columnSet);

        if (columnSet == null)
        {
            foreach (var parentCatDef in catDef.Parents)
            {
                return GetColumnSetForCatDef(parentCatDef);
            }
        }

        return columnSet;
    }
    static public void Add(ColumnSet columnSet)
    {
        foreach (string category in columnSet.categories)
        {
            columnSets.TryAdd(category, columnSet);
        }
    }
}

class ColumnSet
{
    public List<string> categories = ["Root"];
    public List<Column> columns = [
        new LabelColumn(),
        new StatColumn("MaxHitPoints")
        {
            label = "HP",
        },
        new StatColumn("MarketValue")
        {
            label = "$",
        },
        new StatColumn("Mass"),
    ];
}
