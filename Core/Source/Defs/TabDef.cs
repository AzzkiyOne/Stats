using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Stats;

public abstract class TabDef :
    Def
{
    public string? iconPath;
    public Texture2D Icon { get; private set; } = BaseContent.BadTex;
    public Color iconColor = Color.white;
    public float iconScale = 1f;

    public abstract MainTabWindowTab MakeTab();

    public override void ResolveReferences()
    {
        base.ResolveReferences();

        LongEventHandler.ExecuteWhenFinished(ResolveIcon);
    }

    private void ResolveIcon()
    {
        if (iconPath?.Length > 0)
        {
            Icon = ContentFinder<Texture2D>.Get(iconPath);
        }
    }
}
