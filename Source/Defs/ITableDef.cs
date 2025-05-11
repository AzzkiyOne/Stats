using System.Collections.Generic;
using Stats.Widgets;
using UnityEngine;

namespace Stats;

public interface ITableDef
{
    Texture2D Icon { get; }
    Color IconColor { get; }
    ITableWidget Widget { get; }
}

public interface ITableDef<TObject> : ITableDef
{
    List<IColumnDef<TObject>> Columns { get; }
    TableWorker<TObject> Worker { get; }
}
