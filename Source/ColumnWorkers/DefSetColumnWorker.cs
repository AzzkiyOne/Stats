using System;
using System.Collections.Generic;
using System.Linq;
using Stats.Widgets;
using Verse;

namespace Stats;

public abstract class DefSetColumnWorker<TObject, TValue> : ColumnWorker<TObject> where TValue : Def
{
    protected DefSetColumnWorker(ColumnDef columnDef, bool cached = true) : base(columnDef, ColumnCellStyle.String)
    {
        GetCachedValue = GetValue;

        if (cached)
        {
            GetCachedValue = GetCachedValue.Memoized();
        }

        GetDefLabels = FunctionExtensions.Memoized((TObject @object) =>
        {
            var defs = GetCachedValue(@object);

            if (defs.Count == 0)
            {
                return "";
            }

            var defLabels = defs
                .OrderBy(def => def.label)
                .Select(def => def.LabelCap);

            return string.Join("\n", defLabels);
        });
    }
    protected readonly Func<TObject, HashSet<TValue>> GetCachedValue;
    protected abstract HashSet<TValue> GetValue(TObject @object);
    private readonly Func<TObject, string> GetDefLabels;
    public override Widget? GetTableCellWidget(TObject @object)
    {
        var defLabels = GetDefLabels(@object);

        if (defLabels.Length == 0)
        {
            return null;
        }

        return new Label(defLabels);
    }
    public override FilterWidget<TObject> GetFilterWidget(IEnumerable<TObject> tableRecords)
    {
        return Make.MTMDefFilter(GetCachedValue, tableRecords);
    }
    public sealed override int Compare(TObject object1, TObject object2)
    {
        return GetDefLabels(object1).CompareTo(GetDefLabels(object2));
    }
}
