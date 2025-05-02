using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.RelationalOperators;
using Stats.RelationalOperators.Set;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Stats.Widgets.FilterWidgets;

// TODO: I have a strong suspicion that enumerables returned by lhs function will be accessed through
// IEnumerable<T> interface, which is not ideal. Instead we could change generic parameters to something
// like this: <TLhs, TElement>. Although, this may have a negative impact on ergonomics.

public sealed class ManyToManyOptionsFilter<TOption> : OptionsFilter<IEnumerable<TOption>, TOption>
{
    private static readonly RelationalOperator<IEnumerable<TOption>, HashSet<TOption>>[] Operators =
        [
            IntersectsWith<IEnumerable<TOption>, HashSet<TOption>, TOption>.Instance,
            IsSubsetOf<IEnumerable<TOption>, HashSet<TOption>, TOption>.Instance,
            IsNotSubsetOf<IEnumerable<TOption>, HashSet<TOption>, TOption>.Instance,
            IsSupersetOf<IEnumerable<TOption>, HashSet<TOption>, TOption>.Instance,
            IsNotSupersetOf<IEnumerable<TOption>, HashSet<TOption>, TOption>.Instance,
        ];
    public ManyToManyOptionsFilter(
        Func<ThingAlike, IEnumerable<TOption>> lhs,
        IEnumerable<TOption> options,
        OptionWidgetFactory<TOption> makeOptionWidget
    ) : this(new(lhs, []), Operators, options, makeOptionWidget)
    {
    }
    private ManyToManyOptionsFilter(
        FilterExpression<IEnumerable<TOption>, HashSet<TOption>> state,
        IEnumerable<RelationalOperator<IEnumerable<TOption>, HashSet<TOption>>> operators,
        IEnumerable<TOption> options,
        OptionWidgetFactory<TOption> makeOptionWidget
    ) : base(state, operators, options, makeOptionWidget)
    {
    }
    public override FilterWidget Clone()
    {
        return new ManyToManyOptionsFilter<TOption>(State, Operators, Options, MakeOptionWidget);
    }
}

public sealed class OneToManyOptionsFilter<TOption> : OptionsFilter<TOption, TOption>
{
    private static readonly RelationalOperator<TOption, HashSet<TOption>>[] Operators =
        [
            IsIn<TOption, HashSet<TOption>>.Instance,
            IsNotIn<TOption, HashSet<TOption>>.Instance,
        ];
    public OneToManyOptionsFilter(
        Func<ThingAlike, TOption> lhs,
        IEnumerable<TOption> options,
        OptionWidgetFactory<TOption> makeOptionWidget
    ) : this(new(lhs, []), Operators, options, makeOptionWidget)
    {
    }
    private OneToManyOptionsFilter(
        FilterExpression<TOption, HashSet<TOption>> state,
        IEnumerable<RelationalOperator<TOption, HashSet<TOption>>> operators,
        IEnumerable<TOption> options,
        OptionWidgetFactory<TOption> makeOptionWidget
    ) : base(state, operators, options, makeOptionWidget)
    {
    }
    public override FilterWidget Clone()
    {
        return new OneToManyOptionsFilter<TOption>(State, Operators, Options, MakeOptionWidget);
    }
}

public abstract class OptionsFilter<TLhs, TOption> : FilterWidget<TLhs, HashSet<TOption>>
{
    protected IEnumerable<TOption> Options { get; }
    protected OptionWidgetFactory<TOption> MakeOptionWidget { get; }
    private readonly OptionsWindowWidget OptionsWindow;
    private string SelectedItemsCountString;
    protected OptionsFilter(
        FilterExpression<TLhs, HashSet<TOption>> state,
        IEnumerable<RelationalOperator<TLhs, HashSet<TOption>>> operators,
        IEnumerable<TOption> options,
        OptionWidgetFactory<TOption> makeOptionWidget
    ) : base(state, operators)
    {
        Options = options;
        MakeOptionWidget = makeOptionWidget;
        OptionsWindow = new(options, makeOptionWidget, state);
        SelectedItemsCountString = state.Rhs.Count.ToString();
    }
    protected override Vector2 CalcInputFieldSize()
    {
        var size = Text.CalcSize(SelectedItemsCountString);
        size.x += Globals.GUI.EstimatedInputFieldInnerPadding * 2f;

        return size;
    }
    protected override void DrawInputField(Rect rect)
    {
        if (Widgets.Draw.ButtonTextSubtle(rect, SelectedItemsCountString, Globals.GUI.EstimatedInputFieldInnerPadding))
        {
            Find.WindowStack.Add(OptionsWindow);
        }
    }
    protected override void HandleStateChange(FilterExpression state)
    {
        SelectedItemsCountString = State.Rhs.Count.ToString();

        base.HandleStateChange(state);
    }

    // TODO: Implement scroll.
    private sealed class OptionsWindowWidget : Window
    {
        protected override float Margin => 0f;
        public override Vector2 InitialSize => OptionsList.GetSize();
        private readonly Widget OptionsList;
        public OptionsWindowWidget(
            IEnumerable<TOption> options,
            OptionWidgetFactory<TOption> makeOptionWidget,
            FilterExpression<TLhs, HashSet<TOption>> state
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
                    if (Event.current.type == EventType.Repaint && state.Rhs.Contains(option))
                    {
                        Verse.Widgets.DrawHighlightSelected(rect);
                    }
                }
                void handleOptionClick()
                {
                    if (state.Rhs.Contains(option))
                    {
                        state.Rhs.Remove(option);
                    }
                    else
                    {
                        state.Rhs.Add(option);
                    }

                    state.NotifyChanged();
                }

                Widget optionWidget = makeOptionWidget(option)
                    .PaddingAbs(Globals.GUI.PadSm, Globals.GUI.PadXs)
                    .WidthRel(1f)
                    .HoverShiftHor(4f);
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
                .PaddingAbs(1f)// TODO: Make borders affect widget's size?
                .Border(borderColor)
                .Background(Verse.Widgets.WindowBGFillColor);
        }
        public override void DoWindowContents(Rect rect)
        {
            var origGUIOpacity = Globals.GUI.Opacity;
            var origGUIColor = GUI.color;

            DoFadeEffect(rect);

            OptionsList.Draw(rect, rect.size);

            Globals.GUI.Opacity = origGUIOpacity;
            GUI.color = origGUIColor;
        }
        private void DoFadeEffect(Rect rect)
        {
            rect = rect.ContractedBy(-5f);

            const float maxAllovedMouseDistFromRect = 95f;

            if (rect.Contains(Event.current.mousePosition) == false)
            {
                var mouseDistFromRect = GenUI.DistFromRect(rect, Event.current.mousePosition);

                Globals.GUI.Opacity = 1f - mouseDistFromRect / maxAllovedMouseDistFromRect;
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
            var position = UI.MousePositionOnUIInverted;

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

public delegate Widget OptionWidgetFactory<T>(T option);
