using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using Verse;

namespace Stats;

public sealed class Widget_FilterInput_Str
    : Widget,
      IWidget_FilterInput
{
    protected override Vector2 Size { get; set; }
    private readonly ThingMatcher_Str _ThingMatcher;
    public IThingMatcher ThingMatcher => _ThingMatcher;
    private readonly FloatMenu OperatorsMenu;
    private const string Description = "Use \",\" to search by multiple terms.";
    public Widget_FilterInput_Str(ThingMatcher_Str thingMatcher)
    {
        _ThingMatcher = thingMatcher;
        var menuOptions = new List<FloatMenuOption>(Operatos.Count);
        foreach (var op in Operatos)
        {
            var optText = op.ToString();
            void optCb() => _ThingMatcher.Operator = op;
            menuOptions.Add(new FloatMenuOption(optText, optCb));
        }
        OperatorsMenu = new FloatMenu(menuOptions);
        Size = GetSize();
        thingMatcher.OnChange += UpdateSize;
    }
    public override Vector2 GetSize()
    {
        if (_ThingMatcher.Operator is Op_Str_Any)
        {
            return Text.CalcSize(_ThingMatcher.Operator.ToString());
        }

        var size = Text.CalcSize(
            _ThingMatcher.Operator.ToString() + _ThingMatcher.Value
        );
        size.x += Constants.EstimatedInputFieldInnerPadding * 2;

        return size;
    }
    protected override void DrawContent(Rect rect)
    {
        if
        (
            Widgets.ButtonTextSubtle(
                _ThingMatcher.Operator is Op_Str_Any
                    ? rect
                    : rect.CutByX(rect.height),
                _ThingMatcher.Operator.ToString()
            )
        )
        {
            Find.WindowStack.Add(OperatorsMenu);
        }

        if (_ThingMatcher.Operator is not Op_Str_Any)
        {
            _ThingMatcher.Value = Widgets.TextField(rect, _ThingMatcher.Value);
            TooltipHandler.TipRegion(rect, Description);
        }
    }
    public IWidget_FilterInput Clone()
    {
        return new Widget_FilterInput_Str(_ThingMatcher);
    }

    public sealed class ThingMatcher_Str
        : ThingMatcher<string>
    {
        public ThingMatcher_Str(Func<ThingRec, string> valueFunc)
            : base("", Op_Str_Any.Instance, valueFunc)
        {
        }
    }

    private sealed class Op_Str_Any
        : IBinaryOp<string>
    {
        private Op_Str_Any() { }
        public bool Eval(string lhs, string rhs) => true;
        public override string ToString() => "Any";
        public static IBinaryOp<string> Instance { get; } = new Op_Str_Any();
    }

    private sealed class Op_Str_Contains
        : IBinaryOp<string>
    {
        private Op_Str_Contains() { }
        public bool Eval(string lhs, string rhs) =>
            rhs
            .Split(',')
            .Any(s => lhs.Contains(s, StringComparison.CurrentCultureIgnoreCase));
        public override string ToString() => "~=";
        public static IBinaryOp<string> Instance { get; } = new Op_Str_Contains();
    }

    private sealed class Op_Str_ContainsNot
        : IBinaryOp<string>
    {
        private Op_Str_ContainsNot() { }
        public bool Eval(string lhs, string rhs) =>
            !rhs
            .Split(',')
            .Any(s => lhs.Contains(s, StringComparison.CurrentCultureIgnoreCase));
        public override string ToString() => "!~=";
        public static IBinaryOp<string> Instance { get; } = new Op_Str_ContainsNot();
    }

    private static readonly ReadOnlyCollection<IBinaryOp<string>> Operatos =
        new([
            Op_Str_Any.Instance,
            Op_Str_Contains.Instance,
            Op_Str_ContainsNot.Instance,
        ]);
}
