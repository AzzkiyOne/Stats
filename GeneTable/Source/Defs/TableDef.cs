using System;
using System.Collections.Generic;
using Stats.Defs;
using Stats.GeneTable.DefOfs;
using Stats.TableWorkers;
using Stats.Widgets;
using Verse;

namespace Stats.GeneTable.Defs;

public sealed class TableDef : Stats.Defs.TableDef, ITableDef<GeneDef>
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

        Worker = workerFactory(this);
        Columns = [ColumnDefOf.Label_GeneTableColumn, .. columns];
    }
}
