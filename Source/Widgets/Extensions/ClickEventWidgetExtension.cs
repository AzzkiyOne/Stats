using System;
using UnityEngine;
using Verse.Sound;

namespace Stats.Widgets.Extensions;

public sealed class ClickEventWidgetExtension
    : WidgetExtension
{
    private readonly Action Action;
    private readonly bool PlaySound;
    internal ClickEventWidgetExtension(
        IWidget widget,
        Action action,
        bool playSound = true
    ) : base(widget)
    {
        Action = action;
        PlaySound = playSound;
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
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
