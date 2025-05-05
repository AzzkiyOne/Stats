using System;
using System.Collections.Generic;
using Stats.Widgets;
using Verse;

namespace Stats.GeneTable;

public sealed class TableDef : Stats.TableDef, ITableDef<GeneDef>
{
#pragma warning disable CS8618
    public Func<TableDef, TableWorker<GeneDef>> workerFactory;
    public TableWorker<GeneDef> Worker { get; private set; }
    public List<ColumnDef> columns;
    public List<IColumnDef<GeneDef>> Columns { get; private set; }
#pragma warning restore CS8618
    private ITableWidget? _widget;
    public override ITableWidget Widget => _widget ??= new GenericTable<GeneDef>(this);
    public override void ResolveReferences()
    {
        base.ResolveReferences();

        Columns = [ColumnDefOf.Label_GeneTableColumn, .. columns];

        LongEventHandler.ExecuteWhenFinished(() => Worker = workerFactory(this));
    }
}
