using System;
using System.Collections.Generic;
using Stats.Widgets;
using Verse;

namespace Stats.ThingTable;

public sealed class TableDef : Stats.TableDef, ITableDef<ThingAlike>
{
#pragma warning disable CS8618
    public Func<TableDef, TableWorker<ThingAlike>> workerFactory;
    public TableWorker<ThingAlike> Worker { get; private set; }
    public List<ColumnDef> columns;
    public List<IColumnDef<ThingAlike>> Columns { get; private set; }
#pragma warning restore CS8618
    private ITableWidget? _widget;
    public override ITableWidget Widget => _widget ??= new GenericTable<ThingAlike>(this);
    public override void ResolveReferences()
    {
        base.ResolveReferences();

        Columns = [ColumnDefOf.Label_ThingTableColumn, .. columns];

        LongEventHandler.ExecuteWhenFinished(() => Worker = workerFactory(this));
    }
}
