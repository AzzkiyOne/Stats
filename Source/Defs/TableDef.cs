using System;
using System.Collections.Generic;
using RimWorld;
using Stats.TableWorkers;
using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats;

public sealed class TableDef
    : Def
{
#pragma warning disable CS8618
    public List<ColumnDef> columns;
#pragma warning restore CS8618
#pragma warning disable CS8618
    public Type workerClass;
#pragma warning restore CS8618
    public string? iconPath;
    public ThingDef? iconThingDef;
#pragma warning disable CS8618
    internal ITableWorker Worker { get; private set; }
#pragma warning restore CS8618
    internal Texture2D Icon { get; private set; } = BaseContent.BadTex;
    internal Color IconColor { get; private set; } = Color.white;
    private ThingTable? _widget;
    internal ThingTable Widget => _widget ??= new(this);
    public override void PostLoad()
    {
        base.PostLoad();

        LongEventHandler.ExecuteWhenFinished(ResolveIcon);
    }
    public override void ResolveReferences()
    {
        base.ResolveReferences();

        Worker = (ITableWorker)Activator.CreateInstance(workerClass);
        Worker.TableDef = this;
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
