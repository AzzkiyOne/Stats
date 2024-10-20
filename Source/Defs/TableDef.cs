using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats;

public class TableDef : Def
{
    public TableDef? parent;
    public Func<ThingDef, bool>? filter;
    public List<ColumnDef> columns;
    public string? iconPath;
    public ThingDef? iconThingDef;
    private TableWidget_Main? _widget;
    internal TableWidget_Main Widget => _widget ??= new(this);
    private Texture2D _iconTex = BaseContent.BadTex;
    public Texture2D Icon
    {
        get
        {
            if (iconPath?.Length > 0)
            {
                _iconTex = ContentFinder<Texture2D>.Get(iconPath);
            }

            return _iconTex;
        }
    }
    public string Path { get; private set; }
    public override void ResolveReferences()
    {
        base.ResolveReferences();

        Path = LabelCap;

        var curParent = parent;

        while (curParent != null)
        {
            // Maybe better use StringBuilder.
            Path = $"{curParent.LabelCap} / " + Path;
            curParent = curParent.parent;
        }
    }
}
