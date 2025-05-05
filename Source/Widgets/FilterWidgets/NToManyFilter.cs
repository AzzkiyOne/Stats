using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Stats.Widgets.FilterWidgets;

public abstract class NToManyFilter<TObject, TExprLhs, TOption> : FilterWidgetWithInputField<TObject, TExprLhs, HashSet<TOption>>
{
    new protected NtMExpression Expression { get; }
    protected IEnumerable<TOption> Options { get; }
    protected OptionWidgetFactory MakeOptionWidget { get; }
    private readonly OptionsWindowWidget OptionsWindow;
    protected NToManyFilter(
        NtMExpression expression,
        IEnumerable<TOption> options,
        OptionWidgetFactory makeOptionWidget
    ) : base(expression)
    {
        Expression = expression;
        Options = options;
        MakeOptionWidget = makeOptionWidget;
        OptionsWindow = new(options, makeOptionWidget, expression);
    }
    protected sealed override void DrawInputField(Rect rect)
    {
        const float horPad = Globals.GUI.EstimatedInputFieldInnerPadding;

        if (Widgets.Draw.ButtonTextSubtle(rect, base.Expression.RhsString, horPad))
        {
            Find.WindowStack.Add(OptionsWindow);
        }
    }

    protected abstract class NtMExpression : GenExpression
    {
        public sealed override string RhsString => Rhs.Count.ToString();
        public NtMExpression(Func<TObject, TExprLhs> lhs) : base(lhs, [])
        {
        }
        public sealed override void Clear()
        {
            base.Clear();

            Rhs.Clear();
            // TODO: NotifyChanged()?
        }
    }

    // TODO: Implement scroll.
    private sealed class OptionsWindowWidget : Window
    {
        protected override float Margin => 0f;
        public override Vector2 InitialSize => OptionsList.GetSize();
        private readonly Widget OptionsList;
        public OptionsWindowWidget(
            IEnumerable<TOption> options,
            OptionWidgetFactory makeOptionWidget,
            NtMExpression expression
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
                    if (Event.current.type == EventType.Repaint && expression.Rhs.Contains(option))
                    {
                        Verse.Widgets.DrawHighlightSelected(rect);
                    }
                }
                void handleOptionClick()
                {
                    if (expression.Rhs.Contains(option))
                    {
                        expression.Rhs.Remove(option);
                    }
                    else
                    {
                        expression.Rhs.Add(option);
                    }

                    expression.NotifyChanged();
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

    public delegate Widget OptionWidgetFactory(TOption option);
}
