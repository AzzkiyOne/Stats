using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.RelationalOperators;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Stats.Widgets.FilterWidgets;

public sealed class EnumerableFilter<T> : FilterWidget<IEnumerable<T>, ICollection<T>>
{
    private static readonly RelationalOperator<IEnumerable<T>, ICollection<T>>[] DefaultOperators =
        [
            ContainsAnyElementOf<IEnumerable<T>, ICollection<T>, T>.Instance,
            ContainsAllElementsOf<IEnumerable<T>, ICollection<T>, T>.Instance,
        ];
    private readonly IEnumerable<T> Options;
    private readonly Func<T, Widget> MakeOptionWidget;
    private readonly OptionsWindowWidget OptionsWindow;
    private string SelectedItemsCountString;
    public EnumerableFilter(
        FilterExpression<IEnumerable<T>, ICollection<T>> value,
        IEnumerable<RelationalOperator<IEnumerable<T>, ICollection<T>>> operators,
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
        new FilterExpression<IEnumerable<T>, ICollection<T>>(lhs, []),
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
        private static readonly Color BorderColor = Verse.Widgets.SeparatorLineColor;
        public OptionsWindowWidget(
            IEnumerable<T> options,
            Func<T, Widget> makeOptionWidget,
            FilterExpression<IEnumerable<T>, ICollection<T>> value
        )
        {
            doWindowBackground = false;
            drawShadow = false;
            closeOnClickedOutside = true;

            var optionWidgets = new List<Widget>(options.Count());

            foreach (var option in options)
            {
                var optionWidget = makeOptionWidget(option)
                    .PaddingAbs(Globals.GUI.PadSm, Globals.GUI.PadXs)
                    .WidthRel(1f)
                    .BorderBottom(1f, BorderColor)
                    .Background(rect =>
                    {
                        if (Event.current.type == EventType.Repaint)
                        {
                            if (value.Rhs.Contains(option))
                            {
                                Verse.Widgets.DrawHighlightSelected(rect);
                            }
                        }
                    })
                    .HoverBackground(FloatMenuOption.ColorBGActiveMouseover)
                    .OnClick(() =>
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
                    });

                optionWidgets.Add(optionWidget);
            }

            OptionsList = new VerticalContainer(optionWidgets);
        }
        public override void DoWindowContents(Rect rect)
        {
            var origGUIOpacity = Globals.GUI.opacity;
            var origGUIColor = GUI.color;
            var fadeRect = rect.ContractedBy(-5f);

            if (fadeRect.Contains(Event.current.mousePosition) == false)
            {
                var mouseDistanceFromFadeRect = GenUI.DistFromRect(
                    fadeRect,
                    Event.current.mousePosition
                );

                Globals.GUI.opacity = 1f - mouseDistanceFromFadeRect / 95f;
                GUI.color = GUI.color with { a = GUI.color.a * Globals.GUI.opacity };

                if (mouseDistanceFromFadeRect > 95f)
                {
                    Close(doCloseSound: false);
                    SoundDefOf.FloatMenu_Cancel.PlayOneShotOnCamera();
                    Find.WindowStack.TryRemove(this);
                }
            }

            var backgroundColor = Verse.Widgets.WindowBGFillColor;
            backgroundColor.a *= Globals.GUI.opacity;
            Verse.Widgets.DrawBoxSolid(rect, backgroundColor);

            OptionsList.Draw(rect, rect.size);

            var borderColor = BorderColor;
            borderColor.a *= Globals.GUI.opacity;
            Verse.Widgets.DrawLineHorizontal(rect.x, rect.y, rect.width, borderColor);
            VerticalLine.Draw(rect.x, rect.y, rect.height, borderColor);
            VerticalLine.Draw(rect.xMax - 1f, rect.y, rect.height, borderColor);

            Globals.GUI.opacity = origGUIOpacity;
            GUI.color = origGUIColor;
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
