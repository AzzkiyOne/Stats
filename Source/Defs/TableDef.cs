using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Stats;

public class TableDef : Def
{
    public TableDef? parent;
    public Func<ThingAlike, bool?>? filter;
    public List<ColumnDef> columns;
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
                var tableWidgetColumns = new List<ColumnDef>([ColumnDefOf.Id, .. columns]);
                var curParent = parent;

                while (curParent != null)
                {
                    tableWidgetColumns.AddRange(curParent.columns);

                    curParent = curParent.parent;
                }

                _widget = new(things, tableWidgetColumns);
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
