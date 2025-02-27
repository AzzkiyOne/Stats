using System;
using System.Collections.Generic;
using RimWorld;
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
    internal Color IconColor { get; private set; } = Color.white;
    private TableWidget_Main? _widget;
    internal TableWidget_Main Widget => _widget ??= new(this);
    internal Texture2D Icon { get; private set; } = BaseContent.BadTex;
    private void ResolveIcon()
    {
        if (iconPath?.Length > 0)
        {
            Icon = ContentFinder<Texture2D>.Get(iconPath);
        }
        else if (iconThingDef != null)
        {
            if (iconThingDef.MadeFromStuff)
            {
                var stuff = GenStuff.DefaultStuffFor(iconThingDef);

                Icon = iconThingDef.GetUIIconForStuff(stuff);
                IconColor = iconThingDef.GetColorForStuff(stuff);
            }
            else
            {
                Icon = iconThingDef.uiIcon;
                IconColor = iconThingDef.uiIconColor;
            }
        }
    }
    public override void PostLoad()
    {
        base.PostLoad();

        LongEventHandler.ExecuteWhenFinished(ResolveIcon);
    }
}
