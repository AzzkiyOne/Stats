using System;
using RimWorld;
using Verse;

namespace Stats.GeneDefTable;

[DefOf]
public static class ColumnDefOf
{
    public static ColumnDef GeneLabel;
    static ColumnDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(ColumnDefOf));
    }
}

public class ColumnDef : GenTable.ColumnDef, GenTable.IColumn<GeneDef>
{
    public Type workerClass;
    private ColumnWorker worker;
    public GenTable.ColumnWorker<GeneDef> Worker
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
}
