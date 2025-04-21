using System;
using UnityEngine;
using Verse.Sound;

namespace Stats.Widgets.Comps;

public class AddClickEventHandler
    : WidgetComp
{
    private readonly Action Action;
    private readonly bool PlaySound;
    public AddClickEventHandler(ref IWidget widget, Action action, bool playSound = true)
        : base(ref widget)
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
