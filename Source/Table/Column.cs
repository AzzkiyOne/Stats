using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Stats;

public abstract class Column : Def
{
    public float minWidth = 100f;
    public bool isPinned = false;
}

public class LabelColumnDef : Column
{
    public LabelColumnDef() : base()
    {
        minWidth = 250f;
        isPinned = true;
    }
}
