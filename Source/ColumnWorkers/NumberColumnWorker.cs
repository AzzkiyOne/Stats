using System;
using System.Collections.Generic;
using Stats.Widgets;
using UnityEngine;

namespace Stats;

public abstract class NumberColumnWorker<TObject> : ColumnWorker<TObject>
{
    private readonly Texture2D? Icon = null;
    private readonly Func<TObject, decimal> GetCahcedValue;
    protected NumberColumnWorker(
        bool cached = true,
        Texture2D? icon = null
    ) : base(ColumnCellStyle.Number)
    {
        GetCahcedValue = GetValue;

        if (cached)
        {
            GetCahcedValue = GetCahcedValue.Memoized();
        }

        Icon = icon;
    }
    protected abstract decimal GetValue(TObject @object);
    public sealed override Widget? GetTableCellWidget(TObject @object)
    {
        var value = GetValue(@object);

        if (value == 0m)
        {
            return null;
        }

        var label = new Label(value.ToString());

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
        return new NumberFilter<TObject>(GetCahcedValue);
    }
    public sealed override int Compare(TObject object1, TObject object2)
    {
        return GetCahcedValue(object1).CompareTo(GetCahcedValue(object2));
    }
}
