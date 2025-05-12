using System.Collections.Generic;
using Stats.Widgets;

namespace Stats;

public abstract class TableWorker
{
    public TableDef TableDef { get; }
    internal abstract ObjectTable TableWidget { get; }
    protected TableWorker(TableDef tableDef)
    {
        TableDef = tableDef;
    }
}

public abstract class TableWorker<TObject> : TableWorker
{
    private ObjectTable? _TableWidget;
    // We don't want to create every table widget on the start of the game.
    internal sealed override ObjectTable TableWidget => _TableWidget ??= MakeTableWidget();
    protected abstract IEnumerable<TObject> Records { get; }
    protected TableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    private ObjectTable MakeTableWidget()
    {
        var columnWorkers = new List<ColumnWorker<TObject>>(TableDef.columns.Count);

        foreach (var columnDef in TableDef.columns)
        {
            if (columnDef.Worker is ColumnWorker<TObject> columnWorker)
            {
                columnWorkers.Add(columnWorker);
            }
        }

        return new ObjectTable<TObject>(columnWorkers, Records);
    }
}
