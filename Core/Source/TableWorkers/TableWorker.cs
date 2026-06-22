using System;
using System.Collections.Generic;
using System.Linq;
using Stats.ColumnWorkers;
using Verse;

namespace Stats.TableWorkers;

public abstract class TableWorker
{
    public TableDef Def { get; }
    internal abstract ObjectTable TableWidget { get; }

    protected TableWorker(TableDef def)
    {
        Def = def;
    }
}

public abstract class TableWorker<TRecord> :
    TableWorker
{
    internal sealed override ObjectTable TableWidget => field ??= new ObjectTable<TRecord>(this);
    internal readonly List<ColumnDef> CompatibleColumns;
    public abstract List<TRecord> InitialRecords { get; }

    public abstract event Action<TRecord> OnRecordAdded;
    public abstract event Action<TRecord> OnRecordRemoved;

    protected TableWorker(TableDef def) : base(def)
    {
        List<ColumnDef> compatibleColumns = [];
        List<ColumnDef> columnDefs = DefDatabase<ColumnDef>.AllDefsListForReading;
        int columnDefsCount = columnDefs.Count;
        for (int i = 0; i < columnDefsCount; i++)
        {
            ColumnDef columnDef = columnDefs[i];
            Type workerClass = columnDef.workerClass;
            if ((workerClass.IsGenericTypeDefinition || typeof(ColumnWorker<TRecord>).IsAssignableFrom(workerClass))
                && columnDef.tags.Count != 0
                && columnDef.tags.All(def.columnTags.Contains))// Is table's column tags is superset of column's tags.
            {
                compatibleColumns.Add(columnDef);
            }
        }
        CompatibleColumns = compatibleColumns;
    }
}
