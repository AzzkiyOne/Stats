using System;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Stats;

public class WidgetComp_OnClick
    : WidgetComp
{
    private readonly Action CB;
    private readonly bool PlaySound;
    public WidgetComp_OnClick(IWidget widget, Action cb, bool playSound = true)
        : base(widget)
    {
        CB = cb;
        PlaySound = playSound;
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        if (PlaySound) MouseoverSounds.DoRegion(rect);
        if (GUI.Button(rect, "", Widgets.EmptyStyle)) CB();

        Widget.Draw(rect, containerSize);
    }
}
