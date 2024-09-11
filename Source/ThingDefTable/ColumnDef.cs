using System;
using RimWorld;

namespace Stats.ThingDefTable;

[DefOf]
public static class ColumnDefOf
{
    public static ColumnDef ThingLabel;
    static ColumnDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(ColumnDefOf));
    }
}

public class ColumnDef : GenTable.ColumnDef, GenTable.IColumn, GenTable.IRowKey<ThingAlike>
{
    public StatDef? stat;
    public StatDef? Stat => stat;
    public bool formatValue = true;
    public bool FormatValue => formatValue;
    public Type workerClass = typeof(ColumnWorker_Stat);
    private ColumnWorker worker;
    public GenTable.ColumnWorker<ThingAlike> Worker
    {
        get
        {
            if (worker == null)
            {
                worker = (ColumnWorker)Activator.CreateInstance(workerClass);
                worker.Column = this;
            }

            return worker;
        }
    }
    public override void ResolveReferences()
    {
        if (Stat != null)
        {
            if (string.IsNullOrEmpty(label))
            {
                label = Stat.label;
            }

            if (string.IsNullOrEmpty(description))
            {
                description = Stat.description;
            }
        }

        base.ResolveReferences();
    }
}
