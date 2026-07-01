using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Stats.Tables;
using UnityEngine;
using Verse;

namespace Stats;

public class TableDef : TabDef
{
#pragma warning disable CS8618
    public List<ColumnDef> columns;
    internal List<ColumnDef> CompatibleColumns => field ??=
        DefDatabase<ColumnDef>
        .AllDefsListForReading
        // Is table's column tags is superset of column's tags.
        .Where(columnDef => columnDef.tags.Count != 0 && columnDef.tags.All(columnTags.Contains))
        .ToList();
    public Func<TableDef, MainTabWindowTab> factory;
#pragma warning restore CS8618
    public List<string> columnTags = [];

    public override MainTabWindowTab MakeTab()
    {
        return factory(this);
    }

    public override IEnumerable<string> ConfigErrors()
    {
        foreach (string item in base.ConfigErrors())
        {
            yield return item;
        }

        if (columnTags.Count == 0)
        {
            yield return "no column tags.";
        }
    }
}
