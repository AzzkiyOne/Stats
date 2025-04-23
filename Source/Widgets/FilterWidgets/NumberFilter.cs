using System;
using System.Collections.Generic;
using Stats.RelationalOperators;
using UnityEngine;

namespace Stats.Widgets.FilterWidgets;

public sealed class NumberFilter<T> : FilterWidgetWithInputField<T, T>
    where T : struct, IEquatable<T>, IComparable<T>
{
    private static readonly RelationalOperator<T, T>[] DefaultOperators =
        [
            Any<T, T>.Instance,
            Equals<T, T>.Instance,
            NotEquals<T, T>.Instance,
            GreaterThan<T, T>.Instance,
            LesserThan<T, T>.Instance,
            GreaterThanOrEquals<T, T>.Instance,
            LesserThanOrEquals<T, T>.Instance,
        ];
    private string ValueStrBuffer = "";
    public NumberFilter(
        FilterExpression<T, T> filterExpression,
        IEnumerable<RelationalOperator<T, T>> operators
    ) : base(filterExpression, operators)
    {
    }
    public NumberFilter(Func<ThingAlike, T> valueFunc)
        : this(
            new FilterExpression<T, T>(
                valueFunc,
                default,
                Any<T, T>.Instance,
                Any<T, T>.Instance
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
