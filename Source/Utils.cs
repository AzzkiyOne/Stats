using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using UnityEngine;
using Verse;

namespace Stats;

using StatsReportUtility_StatsToDraw = Func<Def, ThingDef?, IEnumerable<StatDrawEntry>>;

static class StatsReportUtilityAccessTool
{
    static readonly MethodInfo StatsToDraw_MethodInfo = typeof(StatsReportUtility).GetMethod(
        "StatsToDraw",
        BindingFlags.Static | BindingFlags.NonPublic,
        null,
        new Type[] { typeof(Def), typeof(ThingDef) },
        []
    );
    static readonly StatsReportUtility_StatsToDraw _StatsToDraw;
    static StatsReportUtilityAccessTool()
    {
        _StatsToDraw = (StatsReportUtility_StatsToDraw)StatsToDraw_MethodInfo.CreateDelegate(typeof(StatsReportUtility_StatsToDraw));
    }
    static public IEnumerable<StatDrawEntry> StatsToDraw(Def def, ThingDef? stuff)
    {
        return _StatsToDraw(def, stuff);
    }
}

class Utils
{
    static public IEnumerable<StatDrawEntry> GetAllDefDisplayStats(Def def, ThingDef? stuff = null)
    {
        StatRequest statReq = (def is BuildableDef _def) ? StatRequest.For(_def, stuff) : StatRequest.ForEmpty();

        return def.SpecialDisplayStats(statReq).Concat(
            from statDrawEntry in StatsReportUtilityAccessTool.StatsToDraw(def, stuff)
            where statDrawEntry.ShouldDisplay()
            select statDrawEntry
        );
    }
    static public void DrawLineVertical(float x, float y, float length, Color color)
    {
        var prevColor = GUI.color;
        GUI.color = color;
        Widgets.DrawLineVertical(x, y, length);
        GUI.color = prevColor;
    }
}
