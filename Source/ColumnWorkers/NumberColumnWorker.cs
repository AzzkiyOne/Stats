using System;
using System.Collections.Generic;
using Stats.Widgets;
using UnityEngine;

namespace Stats;

public sealed class NumberColumnWorker<TObject> : ColumnWorker<TObject>
{
    private readonly string UnitOfMeasureString = "";
    private readonly Texture2D? UnitOfMeasureIcon = null;
    private readonly Func<TObject, decimal> GetValue;
    private NumberColumnWorker(Func<TObject, decimal> valueFunction) : base(TableColumnCellStyle.Number)
    {
        GetValue = valueFunction;
    }
    public NumberColumnWorker(Func<TObject, decimal> valueFunction, string unitOfMeasureString = "")
        : this(valueFunction)
    {
        UnitOfMeasureString = unitOfMeasureString;
    }
    public NumberColumnWorker(Func<TObject, decimal> valueFunction, Texture2D unitOfMeasureIcon)
        : this(valueFunction)
    {
        UnitOfMeasureIcon = unitOfMeasureIcon;
    }
    public sealed override Widget? GetTableCellWidget(TObject @object)
    {
        var value = GetValue(@object);

        if (value == 0m)
        {
            return null;
        }

        if (UnitOfMeasureIcon != null)
        {
            return new HorizontalContainer(
                [
                    new Label(value.ToString()).WidthRel(1f),
                    new Icon(UnitOfMeasureIcon)
                ],
                Globals.GUI.PadSm,
                true
            );
        }

        return new Label(value.ToString() + UnitOfMeasureString);
    }
    public sealed override FilterWidget<TObject> GetFilterWidget(IEnumerable<TObject> _)
    {
        return new NumberFilter<TObject>(GetValue);
    }
    public sealed override int Compare(TObject object1, TObject object2)
    {
        return GetValue(object1).CompareTo(GetValue(object2));
    }
}
