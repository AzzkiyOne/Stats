using System;
using UnityEngine;
using Verse.Sound;

namespace Stats.Widgets.Extensions;

public sealed class AddClickEventHandler
    : WidgetExtension
{
    private readonly Action Action;
    private readonly bool PlaySound;
    internal AddClickEventHandler(IWidget widget, Action action, bool playSound = true)
        : base(widget)
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

public static partial class WidgetAPI
{
    public static AddClickEventHandler OnClick(this IWidget widget, Action action)
    {
        return new(widget, action);
    }
}
