using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats.Widgets.FilterWidgets;

public sealed class NumberFilter : FilterWidgetWithInputField<decimal, decimal>
{
    new private readonly NumberExpression Expression;
    private static readonly Color ErrorColor = Color.red.ToTransparent(0.5f);
    public NumberFilter(Func<ThingAlike, decimal> lhs) : this(new NumberExpression(lhs))
    {
    }
    private NumberFilter(NumberExpression expression) : base(expression)
    {
        Expression = expression;
    }
    protected override void DrawInputField(Rect rect)
    {
        if (Expression.InputIsValid == false)
        {
            Verse.Widgets.DrawBoxSolid(rect, ErrorColor);
        }

        Expression.TextFieldText = Verse.Widgets.TextField(rect, Expression.TextFieldText);
    }
    public override FilterWidget Clone()
    {
        return new NumberFilter(Expression);
    }

    private sealed class NumberExpression : GenExpression
    {
        public override IEnumerable<GenOperator> SupportedOperators => [
            Operators.EqualTo.Instance,
            Operators.NotEqualTo.Instance,
            Operators.GreaterThan.Instance,
            Operators.LesserThan.Instance,
            Operators.GreaterThanOrEqualTo.Instance,
            Operators.LesserThanOrEqualTo.Instance,
        ];
        private string _TextFieldText = "";
        public string TextFieldText
        {
            get => _TextFieldText;
            set
            {
                if (_TextFieldText == value)
                {
                    return;
                }

                _TextFieldText = value.Trim();

                if (_TextFieldText.Length == 0)
                {
                    Rhs = 0m;
                    InputIsValid = true;

                    return;
                }

                var numWasParsed = decimal.TryParse(TextFieldText, out var num);

                if (numWasParsed)
                {
                    Rhs = num;
                    InputIsValid = true;
                }
                else
                {
                    InputIsValid = false;
                    // Although operator/rhs haven't changed, we have to emit "on change" event so the
                    // widget will be resized. This is unoptimal, because table's filters will be
                    // re-applied. But given the semantics of the event ("something changed"), is
                    // correct.
                    //
                    // TODO: Think about it more.
                    NotifyChanged();
                }
            }
        }
        public override string RhsString => _TextFieldText;
        public bool InputIsValid { get; private set; } = true;
        public NumberExpression(Func<ThingAlike, decimal> lhs) : base(lhs, 0m)
        {
        }
        public override void Clear()
        {
            base.Clear();

            Rhs = 0m;
            _TextFieldText = "";
        }

        private static class Operators
        {
            public sealed class EqualTo : GenOperator
            {
                private EqualTo() { }
                public override bool Eval(decimal lhs, decimal rhs) => lhs == rhs;
                public override string ToString() => "==";
                public static EqualTo Instance { get; } = new();
            }

            public sealed class NotEqualTo : GenOperator
            {
                private NotEqualTo() { }
                public override bool Eval(decimal lhs, decimal rhs) => lhs != rhs;
                public override string ToString() => "!=";
                public static NotEqualTo Instance { get; } = new();
            }

            public sealed class GreaterThan : GenOperator
            {
                private GreaterThan() { }
                public override bool Eval(decimal lhs, decimal rhs) => lhs > rhs;
                public override string ToString() => ">";
                public static GreaterThan Instance { get; } = new();
            }

            public sealed class LesserThan : GenOperator
            {
                private LesserThan() { }
                public override bool Eval(decimal lhs, decimal rhs) => lhs < rhs;
                public override string ToString() => "<";
                public static LesserThan Instance { get; } = new();
            }

            public sealed class GreaterThanOrEqualTo : GenOperator
            {
                private GreaterThanOrEqualTo() { }
                public override bool Eval(decimal lhs, decimal rhs) => lhs >= rhs;
                public override string ToString() => ">=";
                public static GreaterThanOrEqualTo Instance { get; } = new();
            }

            public sealed class LesserThanOrEqualTo : GenOperator
            {
                private LesserThanOrEqualTo() { }
                public override bool Eval(decimal lhs, decimal rhs) => lhs <= rhs;
                public override string ToString() => "<=";
                public static LesserThanOrEqualTo Instance { get; } = new();
            }
        }
    }
}
