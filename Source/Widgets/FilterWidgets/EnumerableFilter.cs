using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.RelationalOperators;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Stats.Widgets.FilterWidgets;

public sealed class EnumerableFilter<T> : FilterWidget<IEnumerable<T>, HashSet<T>>
{
    private static readonly RelationalOperator<IEnumerable<T>, HashSet<T>>[] DefaultOperators =
        [
            ContainsAnyElementOf<IEnumerable<T>, HashSet<T>, T>.Instance,
            ContainsAllElementsOf<IEnumerable<T>, HashSet<T>, T>.Instance,
        ];
    private readonly IEnumerable<T> Options;
    private readonly Func<T, Widget> MakeOptionWidget;
    private readonly OptionsWindowWidget OptionsWindow;
    private string SelectedItemsCountString;
    public EnumerableFilter(
        FilterExpression<IEnumerable<T>, HashSet<T>> value,
        IEnumerable<RelationalOperator<IEnumerable<T>, HashSet<T>>> operators,
        IEnumerable<T> options,
        Func<T, Widget> makeOptionWidget
    ) : base(value, operators)
    {
        Options = options;
        MakeOptionWidget = makeOptionWidget;
        OptionsWindow = new OptionsWindowWidget(options, makeOptionWidget, value);
        SelectedItemsCountString = value.Rhs.Count.ToString();
    }
    public EnumerableFilter(
        Func<ThingAlike, IEnumerable<T>> lhs,
        IEnumerable<T> options,
        Func<T, Widget> makeOptionWidget
    ) : this(
        new FilterExpression<IEnumerable<T>, HashSet<T>>(lhs, []),
        DefaultOperators,
        options,
        makeOptionWidget
    )
    {
    }
    protected override Vector2 CalcInputFieldSize()
    {
        var size = Text.CalcSize(SelectedItemsCountString);
        size.x *= 1.3f;

        return size;
    }
    protected override void DrawInputField(Rect rect)
    {
        if (Verse.Widgets.ButtonTextSubtle(rect, SelectedItemsCountString))
        {
            Find.WindowStack.Add(OptionsWindow);
        }
    }
    protected override void HandleValueChange(FilterExpression value)
    {
        SelectedItemsCountString = _Value.Rhs.Count.ToString();

        base.HandleValueChange(value);
    }
    public override FilterWidget Clone()
    {
        return new EnumerableFilter<T>(_Value, DefaultOperators, Options, MakeOptionWidget);
    }

    private sealed class OptionsWindowWidget : Window
    {
        protected override float Margin => 0f;
        public override Vector2 InitialSize => OptionsList.GetSize();
        private readonly Widget OptionsList;
        public OptionsWindowWidget(
            IEnumerable<T> options,
            Func<T, Widget> makeOptionWidget,
            FilterExpression<IEnumerable<T>, HashSet<T>> value
        )
        {
            doWindowBackground = false;
            drawShadow = false;
            closeOnClickedOutside = true;

            // TODO: Not ideal.
            var optionsList = options.ToList();
            var optionWidgets = new List<Widget>(optionsList.Count);
            var borderColor = Verse.Widgets.SeparatorLineColor;

            for (int i = 0; i < optionsList.Count; i++)
            {
                var option = optionsList[i];
                void drawOptionBackground(Rect rect)
                {
                    if (Event.current.type == EventType.Repaint && value.Rhs.Contains(option))
                    {
                        Verse.Widgets.DrawHighlightSelected(rect);
                    }
                }
                void handleOptionClick()
                {
                    if (value.Rhs.Contains(option))
                    {
                        value.Rhs.Remove(option);
                    }
                    else
                    {
                        value.Rhs.Add(option);
                    }

                    value.NotifyChanged();
                }

                Widget optionWidget = makeOptionWidget(option)
                    .PaddingAbs(Globals.GUI.PadSm, Globals.GUI.PadXs)
                    .WidthRel(1f);
                if (i < optionsList.Count - 1)
                {
                    optionWidget = optionWidget
                        .BorderBottom(borderColor);
                }
                optionWidget = optionWidget
                    .Background(drawOptionBackground)
                    .HoverBackground(FloatMenuOption.ColorBGActiveMouseover)
                    .OnClick(handleOptionClick);

                optionWidgets.Add(optionWidget);
            }

            OptionsList = new VerticalContainer(optionWidgets)
                .PaddingAbs(1f)// TODO: Make borders affect widget's size.
                .Border(borderColor)
                .Background(Verse.Widgets.WindowBGFillColor);
        }
        public override void DoWindowContents(Rect rect)
        {
            var origGUIOpacity = Globals.GUI.opacity;
            var origGUIColor = GUI.color;

            DoFadeEffect(rect);

            OptionsList.Draw(rect, rect.size);

            Globals.GUI.opacity = origGUIOpacity;
            GUI.color = origGUIColor;
        }
        private void DoFadeEffect(Rect rect)
        {
            rect = rect.ContractedBy(-5f);

            const float maxAllovedMouseDistFromRect = 95f;

            if (rect.Contains(Event.current.mousePosition) == false)
            {
                var mouseDistFromRect = GenUI.DistFromRect(rect, Event.current.mousePosition);

                Globals.GUI.opacity = 1f - mouseDistFromRect / maxAllovedMouseDistFromRect;
                GUI.color = GUI.color.AdjustedForGUIOpacity();

                if (mouseDistFromRect > maxAllovedMouseDistFromRect)
                {
                    Close();
                }
            }
        }
        public override void Close(bool doCloseSound = false)
        {
            SoundDefOf.FloatMenu_Cancel.PlayOneShotOnCamera();
            base.Close(doCloseSound);
        }
        protected override void SetInitialSizeAndPosition()
        {
            Vector2 position = UI.MousePositionOnUIInverted;

            if (position.x + InitialSize.x > UI.screenWidth)
            {
                position.x = UI.screenWidth - InitialSize.x;
            }

            if (position.y + InitialSize.y > UI.screenHeight)
            {
                position.y = UI.screenHeight - InitialSize.y;
            }

            windowRect = new Rect(position, InitialSize);
        }
    }
}
