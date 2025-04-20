using System;
using System.Collections.Generic;
using System.Linq;
using Stats.Widgets.Comps;
using Stats.Widgets.Comps.Size;
using Stats.Widgets.Comps.Size.Constraints;
using Stats.Widgets.Containers;
using Stats.Widgets.Misc;
using Stats.Widgets.Table.Filters.Operators.Enumerable;
using Stats.Widgets.Table.Filters.Operators.Generic;
using UnityEngine;
using Verse;

namespace Stats.Widgets.Table.Filters.Widgets;

public class Widget_FilterInput_Enum<T>
    : Widget,
      IWidget_FilterInput
{
    protected override Vector2 Size { get; set; }
    private readonly ThingMatcher<IEnumerable<T>> _ThingMatcher;
    public IThingMatcher ThingMatcher => _ThingMatcher;
    private readonly FloatMenu OperatorsMenu;
    private readonly IEnumerable<T> Options;
    private readonly Func<T, IWidget> MakeOptionWidget;
    private readonly OptionsWindow _OptionsWindow;
    public Widget_FilterInput_Enum(
        ThingMatcher<IEnumerable<T>> thingMatcher,
        IEnumerable<T> options,
        Func<T, IWidget> makeOptionWidget
    )
    {
        Options = options;
        MakeOptionWidget = makeOptionWidget;
        var optionWidgets = new List<IWidget>();
        foreach (var option in options)
        {
            IWidget optionWidget = MakeOptionWidget(option);
            new WidgetComp_Size_Inc_Abs(ref optionWidget, 5f, 3f);
            new WidgetComp_Width_Rel(ref optionWidget, 1f);
            new WidgetComp_Bg_Tex_Hover(ref optionWidget, TexUI.HighlightTex);
            //new WidgetComp_Color_Hover(ref optionWidget, FloatMenuOption.ColorBGActiveMouseover);
            new WidgetComp_OnClick(ref optionWidget, () =>
            {
                if (_ThingMatcher.Value.Contains(option))
                {
                    _ThingMatcher.Value = _ThingMatcher.Value.Where(opt => opt.Equals(option) == false);
                }
                else
                {
                    _ThingMatcher.Value = [.. _ThingMatcher.Value, option];
                }
            });
            new WidgetComp_Generic(ref optionWidget, rect =>
            {
                if (Event.current.type == EventType.Repaint)
                {
                    if (_ThingMatcher.Value.Contains(option))
                    {
                        Verse.Widgets.DrawHighlightSelected(rect);
                    }

                    Verse.Widgets.DrawLineHorizontal(rect.x, rect.yMax - 1f, rect.width, Verse.Widgets.SeparatorLineColor);
                }
            });

            optionWidgets.Add(optionWidget);
        }
        IWidget optionsContainer = new Widget_Container_Ver(optionWidgets);
        _OptionsWindow = new OptionsWindow(optionsContainer);
        _ThingMatcher = thingMatcher;
        IRelationalOperator<IEnumerable<T>>[] operators = [
            Any<IEnumerable<T>>.Instance,
            Contains_Any<T>.Instance,
            Contains_All<T>.Instance,
        ];
        var menuOptions = new List<FloatMenuOption>(operators.Length);
        foreach (var op in operators)
        {
            var optText = op.ToString();
            void optCb() => _ThingMatcher.Operator = op;
            menuOptions.Add(new FloatMenuOption(optText, optCb));
        }
        OperatorsMenu = new FloatMenu(menuOptions);
        Size = GetSize();
        thingMatcher.OnChange += HandleThingMatcherOnChange;
    }
    public Widget_FilterInput_Enum(
        Func<ThingRec, IEnumerable<T>> valueFunc,
        IEnumerable<T> options,
        Func<T, IWidget> makeOptionWidget
    ) : this(new ThingMatcher<IEnumerable<T>>(valueFunc, []), options, makeOptionWidget)
    {
    }
    private void HandleThingMatcherOnChange(IThingMatcher thingMathcer)
    {
        UpdateSize();
    }
    public override Vector2 GetSize()
    {
        if (_ThingMatcher.IsActive == false)
        {
            return Text.CalcSize(_ThingMatcher.Operator.ToString());
        }

        var size = Text.CalcSize(
            _ThingMatcher.Operator.ToString() + _ThingMatcher.Value.Count()
        );

        return size with { x = size.x * 1.3f };
    }
    protected override void DrawContent(Rect rect)
    {
        var opStr = _ThingMatcher.Operator.ToString();

        if
        (
            Verse.Widgets.ButtonTextSubtle(
                _ThingMatcher.IsActive == false
                    ? rect
                    : rect.CutByX(Text.CalcSize(opStr).x * 1.3f),// Bad performance.
                opStr
            )
        )
        {
            Find.WindowStack.Add(OperatorsMenu);
        }

        if (_ThingMatcher.IsActive)
        {
            _OptionsWindow.windowRect.x = UI.GUIToScreenRect(rect).xMax;
            _OptionsWindow.windowRect.y = UI.GUIToScreenRect(rect).y;

            // !!! Count().ToString() will produce temporary string objects.
            if (Verse.Widgets.ButtonTextSubtle(rect, _ThingMatcher.Value.Count().ToString()))
            {
                Find.WindowStack.Add(_OptionsWindow);
            }
        }
    }
    public IWidget_FilterInput Clone()
    {
        return new Widget_FilterInput_Enum<T>(_ThingMatcher, Options, MakeOptionWidget);
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
            Widget_LineVertical.Draw(rect.x, rect.y, rect.height, Verse.Widgets.SeparatorLineColor);
            Widget_LineVertical.Draw(rect.xMax - 1f, rect.y, rect.height, Verse.Widgets.SeparatorLineColor);

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
