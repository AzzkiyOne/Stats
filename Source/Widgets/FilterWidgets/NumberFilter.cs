using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

public sealed class NumberFilter<TObject> : FilterWidgetWithInputField<TObject, decimal, decimal>
{
    new private readonly NumberExpression Expression;
    private static readonly Color ErrorColor = Color.red.ToTransparent(0.5f);
    public NumberFilter(Func<TObject, decimal> lhs) : this(new NumberExpression(lhs))
    {
    }
    private NumberFilter(NumberExpression expression) : base(expression)
    {
        Expression = expression;
    }
    protected override Vector2 CalcInputFieldContentSize()
    {
        return Text.CalcSize(Expression.TextFieldText);
    }
    protected override void DrawInputField(Rect rect)
    {
        if (Expression.InputIsValid == false)
        {
            Verse.Widgets.DrawBoxSolid(rect, ErrorColor);
        }

        Expression.TextFieldText = Verse.Widgets.TextField(rect, Expression.TextFieldText);
    }
    public override FilterWidget<TObject> Clone()
    {
        return new NumberFilter<TObject>(Expression);
    }

    private sealed class NumberExpression : GenExpression
    {
        public override IEnumerable<GenericOperator> SupportedOperators { get; } = [
            Operators.IsEqualTo.Instance,
            Operators.IsNotEqualTo.Instance,
            Operators.IsGreaterThan.Instance,
            Operators.IsLesserThan.Instance,
            Operators.IsGreaterThanOrEqualTo.Instance,
            Operators.IsLesserThanOrEqualTo.Instance,
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
        public bool InputIsValid { get; private set; } = true;
        public NumberExpression(Func<TObject, decimal> lhs) : base(lhs, 0m)
        {
        }
        public override void Reset()
        {
            base.Reset();

            Rhs = 0m;
            _TextFieldText = "";
        }

        private static class Operators
        {
            public sealed class IsEqualTo : GenericOperator
            {
                private IsEqualTo() : base("==") { }
                public override bool Eval(decimal lhs, decimal rhs) => lhs == rhs;
                public static IsEqualTo Instance { get; } = new();
            }

            public sealed class IsNotEqualTo : GenericOperator
            {
                private IsNotEqualTo() : base("!=") { }
                public override bool Eval(decimal lhs, decimal rhs) => lhs != rhs;
                public static IsNotEqualTo Instance { get; } = new();
            }

            public sealed class IsGreaterThan : GenericOperator
            {
                private IsGreaterThan() : base(">") { }
                public override bool Eval(decimal lhs, decimal rhs) => lhs > rhs;
                public static IsGreaterThan Instance { get; } = new();
            }

            public sealed class IsLesserThan : GenericOperator
            {
                private IsLesserThan() : base("<") { }
                public override bool Eval(decimal lhs, decimal rhs) => lhs < rhs;
                public static IsLesserThan Instance { get; } = new();
            }

            public sealed class IsGreaterThanOrEqualTo : GenericOperator
            {
                private IsGreaterThanOrEqualTo() : base(">=") { }
                public override bool Eval(decimal lhs, decimal rhs) => lhs >= rhs;
                public static IsGreaterThanOrEqualTo Instance { get; } = new();
            }

            public sealed class IsLesserThanOrEqualTo : GenericOperator
            {
                private IsLesserThanOrEqualTo() : base("<=") { }
                public override bool Eval(decimal lhs, decimal rhs) => lhs <= rhs;
                public static IsLesserThanOrEqualTo Instance { get; } = new();
            }
        }
    }
}
