using System;
using RimWorld;

namespace Stats.ThingDefTable;

[DefOf]
public static class ColumnDefOf
{
    public static ColumnDef Label;
    static ColumnDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(ColumnDefOf));
    }
}

public class ColumnDef : GenTable.ColumnDef, GenTable.IColumnDefWithWorker<ThingAlike>
{
    public StatDef? stat;
    public StatDef? Stat => stat;
    public StatDef? useShouldShowFrom = null;
    public StatDef? UseShouldShowFrom => useShouldShowFrom;
    public bool formatValue = true;
    public bool FormatValue => formatValue;
    public Type workerClass = typeof(StatColumnWorker);
    private ColumnWorker worker;
    public GenTable.IColumnWorker<ThingAlike> Worker
    {
        get
        {
            if (worker == null)
            {
                worker = (ColumnWorker)Activator.CreateInstance(workerClass);
                worker.Def = this;
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
