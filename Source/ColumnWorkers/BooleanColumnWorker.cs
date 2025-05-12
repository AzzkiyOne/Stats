using System;
using System.Collections.Generic;
using Stats.Widgets;

namespace Stats;

public abstract class BooleanColumnWorker<TObject> : ColumnWorker<TObject>
{
    private readonly Func<TObject, bool> GetCachedValue;
    protected BooleanColumnWorker(ColumnDef columndef, bool cached = true) : base(columndef, ColumnCellStyle.Boolean)
    {
        GetCachedValue = GetValue;

        if (cached)
        {
            GetCachedValue = GetCachedValue.Memoized();
        }
    }
    protected abstract bool GetValue(TObject @object);
    public sealed override Widget? GetTableCellWidget(TObject @object)
    {
        var value = GetValue(@object);

        if (value == false)
        {
            return null;
        }

        return new SingleElementContainer(
            new Icon(Verse.Widgets.CheckboxOnTex)
                .PaddingRel(0.5f, 0f)
        );
    }
    public sealed override FilterWidget<TObject> GetFilterWidget(IEnumerable<TObject> _)
    {
        return new BooleanFilter<TObject>(GetCachedValue);
    }
    public sealed override int Compare(TObject object1, TObject object2)
    {
        return GetCachedValue(object1).CompareTo(GetCachedValue(object2));
    }
}
