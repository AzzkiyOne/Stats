using System.Collections.Generic;
using RimWorld;
using Stats.TableWorkers;
using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats.Defs;

public abstract class TableDef : Def
{
    public string? iconPath;
    public ThingDef? iconThingDef;
    public Texture2D Icon { get; private set; } = BaseContent.BadTex;
    public Color IconColor { get; private set; } = Color.white;
    public abstract ITableWidget Widget { get; }
    public override void PostLoad()
    {
        base.PostLoad();

        LongEventHandler.ExecuteWhenFinished(ResolveIcon);
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

public interface ITableDef<T>
{
    List<IColumnDef<T>> Columns { get; }
    TableWorker<T> Worker { get; }
}
