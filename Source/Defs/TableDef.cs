using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace Stats;

public class TableDef : Def
{
    public string? iconPath;
    public ThingDef? iconThingDef;
    public Texture2D Icon { get; private set; } = BaseContent.BadTex;
    public Color IconColor { get; private set; } = Color.white;
#pragma warning disable CS8618
    public List<ColumnDef> columns;
    public Type workerClass;
    public TableWorker Worker { get; private set; }
#pragma warning restore CS8618
    public override void ResolveReferences()
    {
        base.ResolveReferences();

        LongEventHandler.ExecuteWhenFinished(() =>
        {
            ResolveIcon();
            Worker = (TableWorker)Activator.CreateInstance(workerClass, this);
        });
    }
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
}
