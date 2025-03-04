using System;
using UnityEngine;
using Verse;

namespace Stats;

public class Widget_FilterInput_Bool : IWidget_FilterInput
{
    private bool? _curValue = null;
    private bool? CurValue
    {
        get => _curValue;
        set
        {
            if (_curValue == value) return;

            _curValue = value;
            WasUpdated = true;
        }
    }
    public bool WasUpdated { get; set; } = false;
    public bool HasValue => CurValue != null;
    private readonly Func<ThingRec, bool> ValueFunc;
    public Widget_FilterInput_Bool(Func<ThingRec, bool> valueFunc)
    {
        ValueFunc = valueFunc;
    }
    public bool Match(ThingRec thing)
    {
        var value = ValueFunc(thing);

        return CurValue switch
        {
            #pragma warning disable format
            null => true,
            _    => value == CurValue,
            #pragma warning restore format
        };
    }
    public void Draw(Rect targetRect)
    {
        switch (CurValue)
        {
            case null:
                if (Widgets.ButtonTextSubtle(targetRect, "Any")) CurValue = true;
                break;
            case true:
                if (Widgets.ButtonImageFitted(targetRect, Widgets.CheckboxOnTex)) CurValue = false;
                break;
            case false:
                if (Widgets.ButtonImageFitted(targetRect, Widgets.CheckboxOffTex)) CurValue = null;
                break;
        }
    }
}
