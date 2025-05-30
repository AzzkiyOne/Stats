﻿using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.ThingTable;
using Stats.Widgets;
using Verse;

namespace Stats.Compat.CE.ThingTable;

public sealed class RangedCaliberColumnWorker : ColumnWorker<ThingAlike>
{
    public RangedCaliberColumnWorker(ColumnDef columnDef) : base(columnDef, ColumnCellStyle.String)
    {
    }
    private static readonly Func<ThingAlike, string?> GetCaliberName =
    FunctionExtensions.Memoized((ThingAlike thing) =>
    {
        var statRequest = StatRequest.For(thing.Def, thing.StuffDef);

        if (StatDefOf.Caliber.Worker.ShouldShowFor(statRequest) == false)
        {
            return null;
        }

        return StatDefOf.Caliber.Worker.GetStatDrawEntryLabel(
            StatDefOf.Caliber,
            StatDefOf.Caliber.Worker.GetValue(statRequest),
            ToStringNumberSense.Absolute,
            statRequest
        );
    });
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        var caliberName = GetCaliberName(thing);

        if (caliberName == null || caliberName.Length == 0)
        {
            return null;
        }

        var statRequest = StatRequest.For(thing.Def, thing.StuffDef);
        var tooltip = StatDefOf.Caliber.Worker.GetExplanationFull(
            statRequest,
            ToStringNumberSense.Absolute,
            StatDefOf.Caliber.Worker.GetValue(statRequest)
        );
        var widget = new Label(caliberName);

        if (tooltip?.Length > 0)
        {
            return widget.Tooltip(tooltip);
        }

        return widget;
    }
    public override FilterWidget<ThingAlike> GetFilterWidget(IEnumerable<ThingAlike> tableRecords)
    {
        return Make.OTMFilter(GetCaliberName, tableRecords);
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return Comparer<string?>.Default.Compare(GetCaliberName(thing1), GetCaliberName(thing2));
    }
}
