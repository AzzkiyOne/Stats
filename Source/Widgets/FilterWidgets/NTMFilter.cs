using System;
using System.Collections.Generic;
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
    private void HandleOptionClick(TOption option)
    {
        if (Rhs.Contains(option))
        {
            Rhs.Remove(option);
        }
        else
        {
            Rhs.Add(option);
        }

        NotifyChanged();
    }

    private sealed class OptionsWindowWidget : Window
    {
        protected override float Margin => 0f;
        public override Vector2 InitialSize => Widget.GetSize();
        private readonly Widget Widget;
        private static readonly Color BorderColor = Verse.Widgets.SeparatorLineColor;
        private const float BorderThickness = 1f;
        private static readonly Color BackgroundColor = Verse.Widgets.WindowBGFillColor;
        private const float OptionHoverHorShiftAmount = 4f;
        private static readonly Color OptionHoverBackgroundColor = FloatMenuOption.ColorBGActiveMouseover;
        private const float OptionHorPad = Globals.GUI.PadSm;
        private const float OptionVerPad = Globals.GUI.PadXs;
        public OptionsWindowWidget(
            IEnumerable<NTMFilterOption<TOption>> options,
            NTMFilter<TObject, TExprLhs, TOption> parent
        )
        {
            doWindowBackground = false;
            drawShadow = false;
            closeOnClickedOutside = true;

            var optionWidgets = new List<Widget>();

            foreach (var option in options)
            {
                Widget optionWidget = option
                    .ToWidget()
                    .PaddingAbs(OptionHorPad, OptionVerPad)
                    .WidthRel(1f)
                    .HoverShiftHor(OptionHoverHorShiftAmount)
                    .Background(rect =>
                    {
                        if (parent.Rhs.Contains(option.Value))
                        {
                            Verse.Widgets.DrawHighlightSelected(rect);
                        }
                    })
                    .HoverBackground(OptionHoverBackgroundColor)
                    .OnClick(() => parent.HandleOptionClick(option.Value));

                if (option.Tooltip?.Length > 0)
                {
                    optionWidget = optionWidget.Tooltip(option.Tooltip);
                }

                if (optionWidgets.Count > 0)
                {
                    optionWidget = optionWidget.BorderTop(BorderThickness, BorderColor);
                }

                optionWidgets.Add(optionWidget);
            }

            var optionsList = new VerticalContainer(optionWidgets)
                .OverflowScroll()
                .SizeRel(1f)
                .BorderTop(BorderThickness, BorderColor);
            var clearSelectionButton = new Label("Clear selection")
                .PaddingAbs(OptionHorPad, OptionVerPad)
                .WidthRel(1f)
                .ToButtonSubtle(() =>
                {
                    parent.Rhs.Clear();
                    parent.NotifyChanged();
                });

            Widget = new VerticalContainer([clearSelectionButton, optionsList], shareFreeSpace: true)
                .Background(BackgroundColor)
                .Border(BorderThickness, BorderColor);
        }
        public override void DoWindowContents(Rect rect)
        {
            var origGUIOpacity = Globals.GUI.Opacity;
            var origGUIColor = GUI.color;

            DoFadeEffect(rect);

            Widget.Draw(rect, rect.size);

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
            var size = InitialSize;
            // Has to be dynamic because the user can change screen size in game.
            var maxHeight = UI.screenHeight / 2.5f;

            if (size.y > maxHeight)
            {
                size.y = maxHeight;
                size.x += GenUI.ScrollBarWidth;
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

public readonly record struct NTMFilterOption<TValue>(TValue Value, string? Label = null, Widget? Icon = null, string? Tooltip = null)
{
    public Widget ToWidget()
    {
        if (Value == null)
        {
            return new Label("<i>Undefined</i>");
        }

        Widget label = new Label(Label ?? "");

        if (Icon != null)
        {
            return new HorizontalContainer([Icon, label], Globals.GUI.PadSm);
        }

        return label;
    }
}
