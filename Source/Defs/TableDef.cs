using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace Stats;

public class TableDef : Def
{
    public string? iconPath;
    public Texture2D Icon { get; private set; } = BaseContent.BadTex;
    public Color iconColor = Color.white;
    public Color IconColor => iconColor;
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
    }
}
