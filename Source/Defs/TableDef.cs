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

                _widget = new(things, [ColumnDefOf.Id, .. columns]);
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
