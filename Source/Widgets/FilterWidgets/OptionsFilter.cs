using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.RelationalOperators;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Stats.Widgets.FilterWidgets;

// TODO: I have a strong suspicion that enumerables returned by lhs function will be accessed through
// IEnumerable<T> interface, which is not ideal. Instead we could change generic parameters to something
// like this: <TLhs, TElement>. Although, this may have a negative impact on ergonomics.

public sealed class ManyToManyOptionsFilter<TOption> : OptionsFilter<IEnumerable<TOption>, TOption>
{
    private static readonly RelationalOperator<IEnumerable<TOption>, HashSet<TOption>>[] DefaultOperators =
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
        Func<TOption, Widget> makeOptionWidget
    ) : this(
        new FilterExpression<
            IEnumerable<TOption>,
            HashSet<TOption>
        >(lhs, []),
        DefaultOperators,
        options,
        makeOptionWidget
    )
    {
    }
    private ManyToManyOptionsFilter(
        FilterExpression<
            IEnumerable<TOption>,
            HashSet<TOption>
        > value,
        IEnumerable<
            RelationalOperator<
                IEnumerable<TOption>,
                HashSet<TOption>
            >
        > operators,
        IEnumerable<TOption> options,
        Func<TOption, Widget> makeOptionWidget
    ) : base(value, operators, options, makeOptionWidget)
    {
    }
    public override FilterWidget Clone()
    {
        return new ManyToManyOptionsFilter<TOption>(_Value, DefaultOperators, Options, MakeOptionWidget);
    }
}

public sealed class OneToManyOptionsFilter<TOption> : OptionsFilter<TOption, TOption>
{
    private static readonly RelationalOperator<TOption, HashSet<TOption>>[] DefaultOperators =
        [
            IsIn<TOption, HashSet<TOption>>.Instance,
            IsNotIn<TOption, HashSet<TOption>>.Instance,
        ];
    public OneToManyOptionsFilter(
        Func<ThingAlike, TOption> lhs,
        IEnumerable<TOption> options,
        Func<TOption, Widget> makeOptionWidget
    ) : this(
        new FilterExpression<TOption, HashSet<TOption>>(lhs, []),
        DefaultOperators,
        options,
        makeOptionWidget
    )
    {
    }
    private OneToManyOptionsFilter(
        FilterExpression<TOption, HashSet<TOption>> value,
        IEnumerable<RelationalOperator<TOption, HashSet<TOption>>> operators,
        IEnumerable<TOption> options,
        Func<TOption, Widget> makeOptionWidget
    ) : base(value, operators, options, makeOptionWidget)
    {
    }
    public override FilterWidget Clone()
    {
        return new OneToManyOptionsFilter<TOption>(_Value, DefaultOperators, Options, MakeOptionWidget);
    }
}

public abstract class OptionsFilter<TLhs, TOption> : FilterWidget<TLhs, HashSet<TOption>>
{
    protected IEnumerable<TOption> Options { get; }
    protected Func<TOption, Widget> MakeOptionWidget { get; }
    private readonly OptionsWindowWidget OptionsWindow;
    private string SelectedItemsCountString;
    protected OptionsFilter(
        FilterExpression<TLhs, HashSet<TOption>> value,
        IEnumerable<RelationalOperator<TLhs, HashSet<TOption>>> operators,
        IEnumerable<TOption> options,
        Func<TOption, Widget> makeOptionWidget
    ) : base(value, operators)
    {
        Options = options;
        MakeOptionWidget = makeOptionWidget;
        OptionsWindow = new OptionsWindowWidget(options, makeOptionWidget, value);
        SelectedItemsCountString = value.Rhs.Count.ToString();
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

    // TODO: Implement scroll.
    private sealed class OptionsWindowWidget : Window
    {
        protected override float Margin => 0f;
        public override Vector2 InitialSize => OptionsList.GetSize();
        private readonly Widget OptionsList;
        public OptionsWindowWidget(
            IEnumerable<TOption> options,
            Func<TOption, Widget> makeOptionWidget,
            FilterExpression<TLhs, HashSet<TOption>> value
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
