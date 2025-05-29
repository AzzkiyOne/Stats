using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Stats.Widgets;

internal abstract class NTMFilter<TObject, TExprLhs, TOption> : FilterWidgetWithInputField<TObject, TExprLhs, HashSet<TOption>>
{
    // TODO: See if IEnumerable is most fitting type here.
    private readonly IEnumerable<NTMFilterOption<TOption>> Options;
    private List<NTMFilterOption<TOption>>? _OptionsList;
    private List<NTMFilterOption<TOption>> OptionsList => _OptionsList ??= Options.ToList();
    private OptionsWindowWidget? _OptionsWindow;
    private OptionsWindowWidget OptionsWindow => _OptionsWindow ??= new(OptionsList, this);
    private TipSignal? _SelectedOptionsTooltip = "";
    private TipSignal SelectedOptionsTooltip
    {
        get
        {
            if (_SelectedOptionsTooltip is TipSignal tipSignal)
            {
                return tipSignal;
            }

            var stringBuilder = new StringBuilder();

            foreach (var option in OptionsList)
            {
                if (Rhs.Contains(option.Value))
                {
                    stringBuilder.AppendLine($"- {option.Label}");
                }
            }

            return (TipSignal)(_SelectedOptionsTooltip = stringBuilder.ToString());
        }
    }
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

        TooltipHandler.TipRegion(rect, SelectedOptionsTooltip);
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
        var optionWasAddedOrRemoved = Rhs.Contains(option)
            ? Rhs.Remove(option)
            : Rhs.Add(option);

        if (optionWasAddedOrRemoved)
        {
            _SelectedOptionsTooltip = null;
        }

        NotifyChanged();
    }

    private sealed class OptionsWindowWidget : Window
    {
        protected override float Margin => 0f;
        public override Vector2 InitialSize => Widget.GetSize();
        private readonly Widget Widget;
        private static readonly Color BorderColor = Verse.Widgets.SeparatorLineColor;
        private static readonly Color BackgroundColor = Verse.Widgets.WindowBGFillColor;
        private const float OptionHoverHorShiftAmount = 4f;
        private static readonly Color OptionHoverBackgroundColor = FloatMenuOption.ColorBGActiveMouseover;
        private const float OptionHorPad = Globals.GUI.Pad;
        private const float OptionVerPad = Globals.GUI.PadXs;
        private const int ColumnCapacity = 15;
        public OptionsWindowWidget(
            List<NTMFilterOption<TOption>> options,
            NTMFilter<TObject, TExprLhs, TOption> parent
        )
        {
            doWindowBackground = false;
            drawShadow = false;
            closeOnClickedOutside = true;

            var columns = new List<List<Widget>>()
            {
                new(ColumnCapacity)
            };
            var currentColumn = columns[0];

            foreach (var option in options)
            {
                if (currentColumn.Count - ColumnCapacity == 0)
                {
                    columns.Add(currentColumn = new(ColumnCapacity));
                }

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
                    .OnClick(() => parent.HandleOptionClick(option.Value))
                    .BorderBottom(BorderColor);

                if (option.Tooltip?.Length > 0)
                {
                    optionWidget = optionWidget.Tooltip(option.Tooltip);
                }

                currentColumn.Add(optionWidget);
            }
            var columnWidgets = new List<Widget>(columns.Count);

            for (int i = 0; i < columns.Count; i++)
            {
                var column = columns[i];
                Widget columnWidget = new VerticalContainer(column)
                    .Background(BackgroundColor)
                    .BorderRight(BorderColor);

                if (i == 0)
                {
                    columnWidget = columnWidget.BorderLeft(BorderColor);
                }

                columnWidgets.Add(columnWidget);
            }

            var clearSelectionButton = new Label("Clear selection")
                .PaddingAbs(OptionHorPad, OptionVerPad)
                .WidthRel(1f)
                .ToButtonSubtle(() =>
                {
                    parent.Rhs.Clear();
                    parent.NotifyChanged();
                });

            Widget = new VerticalContainer(
                [
                    clearSelectionButton,
                    new HorizontalContainer(columnWidgets, stretchItems: true).WidthRel(1f),
                ],
                shareFreeSpace: true
            );
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

public readonly record struct NTMFilterOption<TValue>
{
    public TValue Value { get; }
    public string Label { get; }
    public Widget? Icon { get; }
    public string? Tooltip { get; }
    public NTMFilterOption()
    {
        Value = default;
        Label = "<i>Undefined</i>";
    }
    public NTMFilterOption(TValue value, string label, Widget? icon = null, string? tooltip = null)
    {
        Value = value;
        Label = label;
        Icon = icon;
        Tooltip = tooltip;
    }
    public Widget ToWidget()
    {
        var label = new Label(Label);

        if (Icon != null)
        {
            return new HorizontalContainer([Icon, label], Globals.GUI.PadSm);
        }

        return label;
    }
}
