using System;
using System.Collections.Generic;
using Stats.Widgets;
using UnityEngine;

namespace Stats;

public abstract class NumberColumnWorker<TObject> : ColumnWorker<TObject>
{
    private readonly Texture2D? Icon = null;
    private readonly Func<TObject, decimal> GetCachedValue;
    private readonly string FormatString;
    protected NumberColumnWorker(
        ColumnDef columndef,
        bool cached = true,
        Texture2D? icon = null,
        string formatString = ""
    ) : base(columndef, ColumnCellStyle.Number)
    {
        GetCachedValue = GetValue;

        if (cached)
        {
            GetCachedValue = GetCachedValue.Memoized();
        }

        Icon = icon;
        FormatString = formatString;
    }
    protected abstract decimal GetValue(TObject @object);
    public sealed override Widget? GetTableCellWidget(TObject @object)
    {
        var value = GetValue(@object);

        if (value == 0m)
        {
            return null;
        }

        var label = new Label(value.ToString(FormatString));

        if (Icon != null)
        {
            return new HorizontalContainer(
                [
                    label.WidthRel(1f),
                    new Icon(Icon)
                ],
                Globals.GUI.PadSm,
                true
            );
        }

        return label;
    }
    public sealed override FilterWidget<TObject> GetFilterWidget(IEnumerable<TObject> _)
    {
        return new NumberFilter<TObject>(GetCachedValue);
    }
    public sealed override int Compare(TObject object1, TObject object2)
    {
        return GetCachedValue(object1).CompareTo(GetCachedValue(object2));
    }
}
