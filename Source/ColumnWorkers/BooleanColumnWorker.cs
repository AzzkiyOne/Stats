using System;
using System.Collections.Generic;
using Stats.Widgets;

namespace Stats;

public sealed class BooleanColumnWorker<TObject> : ColumnWorker<TObject>
{
    private readonly Func<TObject, bool> GetValue;
    public BooleanColumnWorker(Func<TObject, bool> valueFunction) : base(ColumnCellStyle.Boolean)
    {
        GetValue = valueFunction;
    }
    public override Widget? GetTableCellWidget(TObject @object)
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
    public override FilterWidget<TObject> GetFilterWidget(IEnumerable<TObject> _)
    {
        return new BooleanFilter<TObject>(GetValue);
    }
    public override int Compare(TObject object1, TObject object2)
    {
        return GetValue(object1).CompareTo(GetValue(object2));
    }
}
