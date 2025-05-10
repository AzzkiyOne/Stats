using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Stats.Widgets;

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
    protected override Vector2 CalcInputFieldContentSize()
    {
        return Text.CalcSize(Expression.Rhs.Count.ToString());
    }
    protected sealed override void DrawInputField(Rect rect)
    {
        const float horPad = Globals.GUI.EstimatedInputFieldInnerPadding;

        if (Widgets.Draw.ButtonTextSubtle(rect, Expression.Rhs.Count.ToString(), horPad))
        {
            Find.WindowStack.Add(OptionsWindow);
        }
    }
    protected override void FocusInputField()
    {
        Find.WindowStack.Add(OptionsWindow);
    }

    protected abstract class NtMExpression : GenExpression
    {
        public NtMExpression(Func<TObject, TExprLhs> lhs) : base(lhs, [])
        {
        }
        public sealed override void Reset()
        {
            base.Reset();

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
        private bool ContentIsTooHigh = false;
        private Vector2 ScrollPosition;
        private static readonly Color BorderColor = Verse.Widgets.SeparatorLineColor;
        private const float BorderThickness = 1f;
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
            // TODO: Copy paste.
            Widget clearSelectionOptionWidget = new Label("<i>Clear selection</i>")
                    .PaddingAbs(Globals.GUI.PadSm, Globals.GUI.PadXs)
                    .WidthRel(1f)
                    .HoverShiftHor(4f);
            if (optionsList.Count > 0)
            {
                clearSelectionOptionWidget = clearSelectionOptionWidget
                    .BorderBottom(BorderThickness, BorderColor);
            }
            clearSelectionOptionWidget = clearSelectionOptionWidget
                .HoverBackground(FloatMenuOption.ColorBGActiveMouseover)
                .OnClick(() =>
                {
                    expression.Rhs.Clear();
                    expression.NotifyChanged();
                });
            var optionWidgets = new List<Widget>(optionsList.Count)
            {
                clearSelectionOptionWidget
            };

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
                        .BorderBottom(BorderThickness, BorderColor);
                }
                optionWidget = optionWidget
                    .Background(drawOptionBackground)
                    .HoverBackground(FloatMenuOption.ColorBGActiveMouseover)
                    .OnClick(handleOptionClick);

                optionWidgets.Add(optionWidget);
            }

            OptionsList = new VerticalContainer(optionWidgets)
                .Background(Verse.Widgets.WindowBGFillColor);
        }
        public override void DoWindowContents(Rect rect)
        {
            var origGUIOpacity = Globals.GUI.Opacity;
            var origGUIColor = GUI.color;

            DoFadeEffect(rect);

            if (ContentIsTooHigh)
            {
                var contentRect = new Rect(Vector2.zero, InitialSize);
                Verse.Widgets.BeginScrollView(rect, ref ScrollPosition, contentRect);

                OptionsList.Draw(contentRect, InitialSize);

                Verse.Widgets.EndScrollView();

                // Adjust rect width for borders, but only if there is vertical scrollbar.
                rect.xMax -= GenUI.ScrollBarWidth;
            }
            else
            {
                OptionsList.Draw(rect, rect.size);
            }

            DrawBorders(rect);

            Globals.GUI.Opacity = origGUIOpacity;
            GUI.color = origGUIColor;
        }
        private void DrawBorders(Rect rect)
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }

            var color = BorderColor.AdjustedForGUIOpacity();
            // Hor:
            // - Top
            var horRect = rect with { height = BorderThickness };
            Verse.Widgets.DrawBoxSolid(horRect, color);
            // - Bottom
            horRect.y = rect.yMax - BorderThickness;
            Verse.Widgets.DrawBoxSolid(horRect, color);
            // Ver:
            // - Left
            var verRect = rect with { width = BorderThickness };
            Verse.Widgets.DrawBoxSolid(verRect, color);
            // - Right
            verRect.x = rect.xMax - BorderThickness;
            Verse.Widgets.DrawBoxSolid(verRect, color);
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
            var size = InitialSize;
            // Has to be dynamic because the user can change screen size in game.
            var maxHeight = UI.screenHeight / 2f;

            if (size.y > maxHeight)
            {
                size.y = maxHeight;
                size.x += GenUI.ScrollBarWidth;
                ContentIsTooHigh = true;
            }
            else
            {
                ContentIsTooHigh = false;
            }

            if (position.x + size.x > UI.screenWidth)
            {
                position.x = UI.screenWidth - size.x;
            }

            if (position.y + size.y > UI.screenHeight)
            {
                position.y = UI.screenHeight - size.y;
            }

            windowRect = new Rect(position, size);
        }
    }

    public delegate Widget OptionWidgetFactory(TOption option);
}
