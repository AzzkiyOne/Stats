using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats;

public interface IColumnDef
{
    string LabelShort { get; }
    TaggedString LabelCap { get; }
    string Description { get; }
    public Texture2D? Icon { get; }
    public Color IconColor { get; }
    public float IconScale { get; }
    // Why not to make it a method of ColumnWorker?
    //
    // It doesn't fit there semantically.
    // ColumnWorker is about data, ColumnDef is about column in general.
    public LabelFormatter LabelFormat { get; }
    public delegate Widget LabelFormatter(IColumnDef columnDef, ColumnCellStyle columnStyle);
}

public interface IColumnDef<TObject> : IColumnDef
{
    ColumnWorker<TObject> Worker { get; }
}
