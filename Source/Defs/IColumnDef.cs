using UnityEngine;
using Verse;

namespace Stats;

public interface IColumnDef
{
    string LabelShort { get; }
    TaggedString LabelCap { get; }
    string Description { get; }
    public Texture2D? Icon { get; }
}

public interface IColumnDef<TObject> : IColumnDef
{
    ColumnWorker<TObject> Worker { get; }
}
