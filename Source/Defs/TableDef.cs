using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats;

public class TableDef : Def
{
    public TableDef? parent;
    public PropDelegate<bool> filter = (ThingDef thingDef, ThingDef? stuffDef) => true;
    public List<ColumnDef> columns;
    internal List<ColumnDef> AllColumns { get; } = [];
    public string icon = "";
    private TableWidget? _widget;
    internal TableWidget Widget => _widget ??= new(this);
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

        AllColumns.Add(ColumnDefOf.Id);
        AllColumns.AddRange(columns);

        var curParent = parent;

        while (curParent != null)
        {
            AllColumns.AddRange(curParent.columns);
            curParent = curParent.parent;
        }
    }
    private Texture2D _iconTex = BaseContent.BadTex;
    internal Texture2D Icon
    {
        get
        {
            if (icon.Length > 0)
            {
                _iconTex = ContentFinder<Texture2D>.Get(icon);
            }

            return _iconTex;
        }
    }
}
