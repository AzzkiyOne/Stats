using System.Collections.Generic;
using Stats.TableWorkers;
using Stats.Widgets;
using UnityEngine;

namespace Stats.Defs;

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
