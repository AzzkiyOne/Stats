using System;
using System.Collections.Generic;
using RimWorld;
using Stats.TableWorkers;
using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats;

public sealed class TableDef : Def
{
#pragma warning disable CS8618
    public List<ColumnDef> columns;
#pragma warning restore CS8618
    public string? iconPath;
    public ThingDef? iconThingDef;
    public Texture2D Icon { get; private set; } = BaseContent.BadTex;
    public Color IconColor { get; private set; } = Color.white;
#pragma warning disable CS8618
    public Func<TableDef, TableWorker> workerFactory;
    public TableWorker Worker { get; private set; }
#pragma warning restore CS8618
    private ThingTable? _widget;
    // TODO: See if we can get rid of null check.
    internal ThingTable Widget => _widget ??= new(this);
    public override void PostLoad()
    {
        base.PostLoad();

        LongEventHandler.ExecuteWhenFinished(ResolveIcon);
    }
    public override void ResolveReferences()
    {
        base.ResolveReferences();

        Worker = workerFactory(this);
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
