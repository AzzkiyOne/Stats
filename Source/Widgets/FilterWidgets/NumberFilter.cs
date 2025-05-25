using System;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

internal sealed class NumberFilter<TObject> : FilterWidgetWithInputField<TObject, decimal, decimal>
{
    private bool InputIsValid = true;
    private string _TextFieldText = "";
    private string TextFieldText
    {
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

            var numWasParsed = decimal.TryParse(_TextFieldText, out var num);

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
    private static readonly Color ErrorColor = Color.red.ToTransparent(0.5f);
    public NumberFilter(Func<TObject, decimal> lhs) : base(lhs, 0m, [
        Operators.IsEqualTo.Instance,
        Operators.IsNotEqualTo.Instance,
        Operators.IsGreaterThan.Instance,
        Operators.IsLesserThan.Instance,
        Operators.IsGreaterThanOrEqualTo.Instance,
        Operators.IsLesserThanOrEqualTo.Instance,
    ])
    {
    }
    protected override Vector2 CalcInputFieldContentSize()
    {
        return Text.CalcSize(_TextFieldText);
    }
    protected override void DrawInputField(Rect rect)
    {
        if (InputIsValid == false)
        {
            Verse.Widgets.DrawBoxSolid(rect, ErrorColor);
        }

        TextFieldText = Verse.Widgets.TextField(rect, _TextFieldText);
    }
    public override void Reset()
    {
        base.Reset();

        Rhs = 0m;
        _TextFieldText = "";
    }

    private static class Operators
    {
        public sealed class IsEqualTo : AbsOperator
        {
            private IsEqualTo() : base("==") { }
            public override bool Eval(decimal lhs, decimal rhs) => lhs == rhs;
            public static IsEqualTo Instance { get; } = new();
        }

        public sealed class IsNotEqualTo : AbsOperator
        {
            private IsNotEqualTo() : base("!=") { }
            public override bool Eval(decimal lhs, decimal rhs) => lhs != rhs;
            public static IsNotEqualTo Instance { get; } = new();
        }

        public sealed class IsGreaterThan : AbsOperator
        {
            private IsGreaterThan() : base(">") { }
            public override bool Eval(decimal lhs, decimal rhs) => lhs > rhs;
            public static IsGreaterThan Instance { get; } = new();
        }

        public sealed class IsLesserThan : AbsOperator
        {
            private IsLesserThan() : base("<") { }
            public override bool Eval(decimal lhs, decimal rhs) => lhs < rhs;
            public static IsLesserThan Instance { get; } = new();
        }

        public sealed class IsGreaterThanOrEqualTo : AbsOperator
        {
            private IsGreaterThanOrEqualTo() : base(">=") { }
            public override bool Eval(decimal lhs, decimal rhs) => lhs >= rhs;
            public static IsGreaterThanOrEqualTo Instance { get; } = new();
        }

        public sealed class IsLesserThanOrEqualTo : AbsOperator
        {
            private IsLesserThanOrEqualTo() : base("<=") { }
            public override bool Eval(decimal lhs, decimal rhs) => lhs <= rhs;
            public static IsLesserThanOrEqualTo Instance { get; } = new();
        }
    }
}
