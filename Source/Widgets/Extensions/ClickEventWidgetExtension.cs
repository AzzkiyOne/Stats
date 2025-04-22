using System;
using UnityEngine;
using Verse.Sound;

namespace Stats.Widgets.Extensions;

public sealed class ClickEventWidgetExtension
    : WidgetDecorator
{
    public override Widget Widget { get; }
    private readonly Action Action;
    private readonly bool PlaySound;
    internal ClickEventWidgetExtension(
        Widget widget,
        Action action,
        bool playSound = true
    )
    {
        Widget = widget;
        Action = action;
        PlaySound = playSound;
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        if (PlaySound)
        {
            MouseoverSounds.DoRegion(rect);
        }

        if (GUI.Button(rect, "", Verse.Widgets.EmptyStyle))
        {
            Action();
        }

        Widget.Draw(rect, containerSize);
    }
}
