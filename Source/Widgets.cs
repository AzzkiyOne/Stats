﻿using System.Reflection;
using UnityEngine;
using Verse;

namespace Stats;

public static class GUIWidgets
{
    private static readonly FieldInfo dialogInfoCardStuffField =
        typeof(Dialog_InfoCard)
        .GetField("stuff", BindingFlags.Instance | BindingFlags.NonPublic);

    public static void DrawLineVertical(float x, float y, float length, Color color)
    {
        using (new ColorCtx(color))
        {
            Widgets.DrawLineVertical(x, y, length);
        }
    }
    public static void DefInfoDialog(Def def, ThingDef? stuff = null)
    {
        var dialog = new Dialog_InfoCard(def);

        if (stuff is not null)
        {
            dialogInfoCardStuffField.SetValue(dialog, stuff);
        }

        Find.WindowStack.Add(dialog);
    }
}
