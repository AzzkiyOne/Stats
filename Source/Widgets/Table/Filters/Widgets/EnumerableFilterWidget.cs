using System;
using System.Collections.Generic;
using System.Linq;
using Stats.Widgets.Comps;
using Stats.Widgets.Comps.Size;
using Stats.Widgets.Comps.Size.Constraints;
using Stats.Widgets.Containers;
using Stats.Widgets.Misc;
using Stats.Widgets.Table.Filters.Operators;
using Stats.Widgets.Table.Filters.Operators.Enumerable;
using Stats.Widgets.Table.Filters.Operators.Generic;
using UnityEngine;
using Verse;

namespace Stats.Widgets.Table.Filters.Widgets;

public sealed class EnumerableFilterWidget<T>
    : FilterWidget<IEnumerable<T>>
{
    private static readonly IRelationalOperator<IEnumerable<T>>[] DefaultOperators =
        [
            Any<IEnumerable<T>>.Instance,
            Contains_Any<T>.Instance,
            Contains_All<T>.Instance,
        ];
    private readonly IEnumerable<T> Options;
    private readonly Func<T, IWidget> MakeOptionWidget;
    private readonly OptionsWindow _OptionsWindow;
    public EnumerableFilterWidget(
        FilterExpression<IEnumerable<T>> filterExpression,
        IEnumerable<IRelationalOperator<IEnumerable<T>>> operators,
        IEnumerable<T> options,
        Func<T, IWidget> makeOptionWidget
    ) : base(filterExpression, operators)
    {
        Options = options;
        MakeOptionWidget = makeOptionWidget;

        var optionWidgets = new List<IWidget>();

        foreach (var option in options)
        {
            IWidget optionWidget = MakeOptionWidget(option);
            new IncreaseSizeByAbs(ref optionWidget, 5f, 3f);
            new SetWidthToRel(ref optionWidget, 1f);
            new DrawTextureOnHover(ref optionWidget, TexUI.HighlightTex);
            //new WidgetComp_Color_Hover(ref optionWidget, FloatMenuOption.ColorBGActiveMouseover);
            new AddClickEventHandler(ref optionWidget, () =>
            {
                if (filterExpression.Value.Contains(option))
                {
                    filterExpression.Value = filterExpression.Value.Where(opt => opt.Equals(option) == false);
                }
                else
                {
                    filterExpression.Value = [.. filterExpression.Value, option];
                }
            });
            new Draw(ref optionWidget, rect =>
            {
                if (Event.current.type == EventType.Repaint)
                {
                    if (filterExpression.Value.Contains(option))
                    {
                        Verse.Widgets.DrawHighlightSelected(rect);
                    }

                    Verse.Widgets.DrawLineHorizontal(rect.x, rect.yMax - 1f, rect.width, Verse.Widgets.SeparatorLineColor);
                }
            });

            optionWidgets.Add(optionWidget);
        }

        IWidget optionsContainer = new VerticalContainer(optionWidgets);
        _OptionsWindow = new OptionsWindow(optionsContainer);
    }
    public EnumerableFilterWidget(
        Func<ThingAlike, IEnumerable<T>> valueFunc,
        IEnumerable<T> options,
        Func<T, IWidget> makeOptionWidget
    ) : this(
        new FilterExpression<IEnumerable<T>>(
            valueFunc,
            [],
            Any<IEnumerable<T>>.Instance,
            Any<IEnumerable<T>>.Instance
        ),
        DefaultOperators,
        options,
        makeOptionWidget
    )
    {
    }
    public override Vector2 GetSize()
    {
        if (_FilterExpression.IsActive == false)
        {
            return Text.CalcSize(_FilterExpression.Operator.ToString());
        }

        var size = Text.CalcSize(
            _FilterExpression.Operator.ToString() + _FilterExpression.Value.Count()
        );

        return size with { x = size.x * 1.3f };
    }
    protected override void DrawContent(Rect rect)
    {
        var opStr = _FilterExpression.Operator.ToString();

        if
        (
            Verse.Widgets.ButtonTextSubtle(
                _FilterExpression.IsActive == false
                    ? rect
                    : rect.CutByX(Text.CalcSize(opStr).x * 1.3f),// Bad performance.
                opStr
            )
        )
        {
            Find.WindowStack.Add(OperatorsMenu);
        }

        if (_FilterExpression.IsActive)
        {
            _OptionsWindow.windowRect.x = UI.GUIToScreenRect(rect).xMax;
            _OptionsWindow.windowRect.y = UI.GUIToScreenRect(rect).y;

            // !!! Count().ToString() will produce temporary string objects.
            if (Verse.Widgets.ButtonTextSubtle(rect, _FilterExpression.Value.Count().ToString()))
            {
                Find.WindowStack.Add(_OptionsWindow);
            }
        }
    }
    public override IFilterWidget Clone()
    {
        return new EnumerableFilterWidget<T>(
            _FilterExpression,
            DefaultOperators,
            Options,
            MakeOptionWidget
        );
    }

    private sealed class OptionsWindow
        : Window
    {
        protected override float Margin => 0f;
        public override Vector2 InitialSize => Widget.GetSize();
        private readonly IWidget Widget;
        public OptionsWindow(IWidget widget)
        {
            Widget = widget;
            doWindowBackground = false;
            drawShadow = false;
            windowRect = new Rect(Vector2.zero, InitialSize);
        }
        public override void DoWindowContents(Rect rect)
        {
            Verse.Widgets.DrawBoxSolid(rect, Verse.Widgets.WindowBGFillColor);
            Verse.Widgets.DrawLineHorizontal(rect.x, rect.y, rect.width, Verse.Widgets.SeparatorLineColor);
            VerticalLine.Draw(rect.x, rect.y, rect.height, Verse.Widgets.SeparatorLineColor);
            VerticalLine.Draw(rect.xMax - 1f, rect.y, rect.height, Verse.Widgets.SeparatorLineColor);

            Widget.DrawIn(rect);
        }
        protected override void SetInitialSizeAndPosition()
        {
            //Vector2 pos = UI.MousePositionOnUIInverted;

            //if (pos.x + InitialSize.x > UI.screenWidth)
            //{
            //    pos.x = UI.screenWidth - InitialSize.x;
            //}

            //if (pos.y + InitialSize.y > UI.screenHeight)
            //{
            //    pos.y = UI.screenHeight - InitialSize.y;
            //}

            //windowRect = new Rect(pos.x, pos.y, InitialSize.x, InitialSize.y);
        }
    }
}
