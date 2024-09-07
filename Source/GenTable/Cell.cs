using System;
using UnityEngine;
using Verse;

namespace Stats.GenTable;

public class Cell
{
    public string Text { get; set; } = "";
    public string Tip { get; set; } = "";
    public Def? Def { get; set; }
    public ThingDef? Stuff { get; set; }
    public Color Color { get; set; } = Color.white;
    public Color? BGColor { get; set; }
    public TextAnchor TextAnchor { get; set; }
    public required IComparable SortValue { get; set; }
}
