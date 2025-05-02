using System;
using System.Collections.Generic;
using Stats.RelationalOperators;
using Stats.RelationalOperators.Decimal;
using UnityEngine;
using Verse;

namespace Stats.Widgets.FilterWidgets;

public sealed class NumberFilter : FilterWidget<NumberFilterState>
{
    private readonly NumberFilterState SharedState;
    public override FilterExpression State => SharedState;
    protected override FloatMenu OperatorsMenu { get; }
    private static readonly Color ErrorColor = Color.red.ToTransparent(0.5f);
    private NumberFilter(NumberFilterState state) : base(state)
    {
        SharedState = state;

        RelationalOperator<decimal, decimal>[] operators =
        [
            EqualTo.Instance,
            NotEqualTo.Instance,
            GreaterThan.Instance,
            LesserThan.Instance,
            GreaterThanOrEqualTo.Instance,
            LesserThanOrEqualTo.Instance,
        ];

        var operatorsMenuOptions = new List<FloatMenuOption>(operators.Length + 1)
        {
            MakeClearStateOperatorsMenuOption(state),
        };

        foreach (var @operator in operators)
        {
            var option = new FloatMenuOption(@operator.ToString(), () => state.Operator = @operator);

            operatorsMenuOptions.Add(option);
        }

        OperatorsMenu = new FloatMenu(operatorsMenuOptions);
    }
    public NumberFilter(Func<ThingAlike, decimal> lhs) : this(new NumberFilterState(lhs))
    {
    }
    protected override void DrawInputField(Rect rect)
    {
        if (SharedState.InputIsValid == false)
        {
            Verse.Widgets.DrawBoxSolid(rect, ErrorColor);
        }

        SharedState.TextFieldText = Verse.Widgets.TextField(rect, SharedState.TextFieldText);
    }
    public override FilterWidget Clone()
    {
        return new NumberFilter(SharedState);
    }
}

public sealed class NumberFilterState : FilterExpression<decimal, decimal>
{
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
            }
        }
    }
    public override string RhsString => _TextFieldText;
    public bool InputIsValid { get; private set; } = true;
    public NumberFilterState(Func<ThingAlike, decimal> lhs) : base(lhs, 0m)
    {
    }
    public override void Clear()
    {
        base.Clear();

        Rhs = 0m;
        _TextFieldText = "";
    }
}
