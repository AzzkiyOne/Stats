using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Stats;

public class ColumnEntry
{
    public ColumnDef def;
    public int order = int.MaxValue;
}

public class TableDef : Def
{
    public TableDef? parent;
    public Func<ThingAlike, bool?>? filter;
    public List<ColumnEntry> columns;
    private TableWidget? _widget;
    internal TableWidget Widget
    {
        get
        {
            if (_widget == null)
            {
                var things = filter == null
                    ? ThingAlike.All
                    : ThingAlike.All.Where(thing => filter(thing) ?? false).ToList();
                var tableColumns = new List<ColumnDef>() { ColumnDefOf.Id };

                columns.SortBy(columnEntry => columnEntry.order);

                foreach (var column in columns)
                {
                    tableColumns.Add(column.def);
                }

                _widget = new(things, tableColumns);
            }

            return _widget;
        }
    }
    internal List<TableDef> Children = [];
    public override void ResolveReferences()
    {
        base.ResolveReferences();

        foreach (var tableDef in DefDatabase<TableDef>.AllDefs)
        {
            if (tableDef.parent == this)
            {
                Children.Add(tableDef);
            }
        }
    }
    public Texture2D Icon = BaseContent.BadTex;
}
