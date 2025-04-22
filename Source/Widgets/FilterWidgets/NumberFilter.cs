using System;
using System.Collections.Generic;
using Stats.RelationalOperators;
using UnityEngine;

namespace Stats.Widgets.FilterWidgets;

public sealed class NumberFilter<T>
    : FilterWidgetWithInputField<T>
    where T :
        struct,
        IEquatable<T>,
        IComparable<T>
{
    private static readonly RelationalOperator<T>[] DefaultOperators =
        [
            Any<T>.Instance,
            Eq<T>.Instance,
            EqNot<T>.Instance,
            Gt<T>.Instance,
            Lt<T>.Instance,
            GtOrEq<T>.Instance,
            LtOrEq<T>.Instance,
        ];
    private string ValueStrBuffer = "";
    public NumberFilter(
        FilterExpression<T> filterExpression,
        IEnumerable<RelationalOperator<T>> operators
    ) : base(filterExpression, operators)
    {
    }
    public NumberFilter(Func<ThingAlike, T> valueFunc)
        : this(
            new FilterExpression<T>(
                valueFunc,
                default,
                Any<T>.Instance,
                Any<T>.Instance
            ),
            DefaultOperators
        )
    {
    }
    protected override void DrawInputField(Rect rect)
    {
        var value = _FilterExpression.Value;
        Verse.Widgets.TextFieldNumeric(rect, ref value, ref ValueStrBuffer);
        _FilterExpression.Value = value;
    }
    public override FilterWidget Clone()
    {
        return new NumberFilter<T>(_FilterExpression, DefaultOperators);
    }
}
