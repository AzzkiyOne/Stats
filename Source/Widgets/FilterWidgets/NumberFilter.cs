using System;
using System.Collections.Generic;
using Stats.RelationalOperators;
using UnityEngine;

namespace Stats.Widgets.FilterWidgets;

public sealed class NumberFilter<T> : FilterWidget<T, T>
    where T : struct, IEquatable<T>, IComparable<T>
{
    private static readonly RelationalOperator<T, T>[] DefaultOperators =
        [
            EqualTo<T, T>.Instance,
            NotEqualTo<T, T>.Instance,
            GreaterThan<T, T>.Instance,
            LesserThan<T, T>.Instance,
            GreaterThanOrEqualTo<T, T>.Instance,
            LesserThanOrEqualTo<T, T>.Instance,
        ];
    private string TextFieldStringBuffer = "";
    private NumberFilter(
        FilterExpression<T, T> value,
        IEnumerable<RelationalOperator<T, T>> operators
    ) : base(value, operators)
    {
    }
    public NumberFilter(Func<ThingAlike, T> lhs)
        : this(new FilterExpression<T, T>(lhs, default), DefaultOperators)
    {
    }
    protected override void DrawInputField(Rect rect)
    {
        var value = _Value.Rhs;
        Verse.Widgets.TextFieldNumeric(rect, ref value, ref TextFieldStringBuffer);
        _Value.Rhs = value;
    }
    public override FilterWidget Clone()
    {
        return new NumberFilter<T>(_Value, DefaultOperators);
    }
}
