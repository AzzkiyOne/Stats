using System;
using System.Collections.Generic;
using Stats.Defs;
using Stats.TableWorkers;
using Stats.ThingTable.DefOfs;
using Stats.ThingTable.TableWorkers;
using Stats.Widgets;

namespace Stats.ThingTable.Defs;

public sealed class TableDef : Stats.Defs.TableDef, ITableDef<ThingAlike>
{
#pragma warning disable CS8618
    public Func<TableDef, TableWorker> workerFactory;
    public TableWorker<ThingAlike> Worker { get; private set; }
    public List<ColumnDef> columns;
    public List<IColumnDef<ThingAlike>> Columns { get; private set; }
#pragma warning restore CS8618
    private ITableWidget? _widget;
    public override ITableWidget Widget => _widget ??= new GenericTable<ThingAlike>(this);
    public override void ResolveReferences()
    {
        base.ResolveReferences();

        Worker = workerFactory(this);
        Columns = [ColumnDefOf.Name, .. columns];
    }
}
