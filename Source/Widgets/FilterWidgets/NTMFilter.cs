using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Stats.Widgets;

internal abstract class NTMFilter<TObject, TExprLhs, TOption> : FilterWidgetWithInputField<TObject, TExprLhs, HashSet<TOption>>
{
    protected IEnumerable<NTMFilterOption<TOption>> Options { get; }
    private OptionsWindowWidget? _OptionsWindow;
    // IEnumerable<TOption> can be a heavy generator, so we defer options window creation.
    private OptionsWindowWidget OptionsWindow => _OptionsWindow ??= new(Options, this);
    protected NTMFilter(
        Func<TObject, TExprLhs> lhs,
        IEnumerable<NTMFilterOption<TOption>> options,
        IEnumerable<AbsOperator> operators
    ) : base(lhs, [], operators)
    {
        Options = options;
    }
    protected override Vector2 CalcInputFieldContentSize()
    {
        return Text.CalcSize(Rhs.Count.ToString());
    }
    protected sealed override void DrawInputField(Rect rect)
    {
        const float horPad = Globals.GUI.EstimatedInputFieldInnerPadding;

        if (Widgets.Draw.ButtonTextSubtle(rect, Rhs.Count.ToString(), horPad))
        {
            Find.WindowStack.Add(OptionsWindow);
        }
    }
    public sealed override void Reset()
    {
        base.Reset();

        Rhs.Clear();
        // TODO: NotifyChanged()?
    }
    protected override void FocusInputField()
    {
        Find.WindowStack.Add(OptionsWindow);
    }

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
            IEnumerable<NTMFilterOption<TOption>> options,
            NTMFilter<TObject, TExprLhs, TOption> parent
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
                    parent.Rhs.Clear();
                    parent.NotifyChanged();
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
                    if (Event.current.type == EventType.Repaint && parent.Rhs.Contains(option.Value))
                    {
                        Verse.Widgets.DrawHighlightSelected(rect);
                    }
                }
                void handleOptionClick()
                {
                    if (parent.Rhs.Contains(option.Value))
                    {
                        parent.Rhs.Remove(option.Value);
                    }
                    else
                    {
                        parent.Rhs.Add(option.Value);
                    }

                    parent.NotifyChanged();
                }

                Widget optionWidget;

                if (option.Value == null)
                {
                    optionWidget = new Label("");
                }
                else if (option.Icon != null)
                {
                    optionWidget = new HorizontalContainer(
                        [option.Icon, new Label(option.Label ?? "")],
                        Globals.GUI.PadSm
                    );
                }
                else
                {
                    optionWidget = new Label(option.Label ?? "");
                }

                optionWidget = optionWidget
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

                if (option.Tooltip?.Length > 0)
                {
                    optionWidget = optionWidget.Tooltip(option.Tooltip);
                }

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
            var maxHeight = UI.screenHeight / 2.5f;

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
}

public readonly record struct NTMFilterOption<TValue>(TValue Value, string? Label = null, Widget? Icon = null, string? Tooltip = null);

