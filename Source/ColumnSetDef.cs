using System.Collections.Generic;
using Verse;

namespace Stats;

static class ColumnSetDB
{
    static ColumnSetDB()
    {
        foreach (var columnSet in DefDatabase<ColumnSetDef>.AllDefs)
        {
            Add(columnSet);
        }
    }
    static private readonly Dictionary<string, ColumnSetDef> columnSets = [];
    static public ColumnSetDef? GetColumnSetForCatDef(ThingCategoryDef catDef)
    {
        columnSets.TryGetValue(catDef.defName, out ColumnSetDef columnSet);

        if (columnSet == null)
        {
            foreach (var parentCatDef in catDef.Parents)
            {
                return GetColumnSetForCatDef(parentCatDef);
            }
        }

        return columnSet;
    }
    static public void Add(ColumnSetDef columnSet)
    {
        foreach (string category in columnSet.categories)
        {
            columnSets.TryAdd(category, columnSet);
        }
    }
}

public class ColumnSetDef : Def
{
    public List<string> categories = [];
    public List<string> columns = [];
}
