using System;
using UnityEngine;

namespace Stats.GenTable;

public class Cell
{
    public string Text { get; init; } = "";
    public string Tip { get; init; } = "";
    public DefReference? DefRef { get; init; }
    public Color Color { get; init; } = Color.white;
    public Color? BGColor { get; init; }
    public TextAnchor TextAnchor { get; init; }
    public required IComparable SortValue { get; init; }
}
