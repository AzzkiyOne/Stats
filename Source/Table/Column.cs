﻿using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Stats;

static class Columns
{
    static public readonly Dictionary<string, IColumn<ThingAlike>> list = [];
    static Columns()
    {
        var labelColumn = new LabelColumn();
        var weaponRangeColumn = new WeaponRangeColumn();

        list.Add(labelColumn.id, labelColumn);
        list.Add(weaponRangeColumn.id, weaponRangeColumn);

        foreach (var statDef in DefDatabase<StatDef>.AllDefs)
        {
            var column = new StatDefColumn(statDef);

            list.TryAdd(column.id, column);
        }
    }
}

public interface IColumn<RowType>
{
    public string id { get; }
    public float minWidth { get; }
    public bool Draw(Rect targetRect, SortDirection? sortDirection = null);
    public ICell GetCellFor(RowType row);
}

public abstract class Column<RowType>(
    string id,
    string? label = null,
    string? description = null,
    float? minWidth = null
) : IColumn<RowType>
{
    public string id { get; } = id;
    public readonly string label = label ?? "";
    public readonly string description = description ?? "";
    public float minWidth { get; } = minWidth ?? 100f;
    // Initialize in constructor with ThingAlikes count?
    private readonly Dictionary<RowType, ICell> cellsCache = [];

    public bool Draw(Rect targetRect, SortDirection? sortDirection = null)
    {
        //Widgets.DrawHighlight(targetRect);
        CellWidgets.Label(targetRect, label);

        if (sortDirection != null)
        {
            var rotationAngle = sortDirection == SortDirection.Ascending ? -90f : 90f;

            Widgets.DrawTextureRotated(
                targetRect.RightPartPixels(Table<ThingAlike>.headersRowHeight),
                TexButton.Reveal,
                rotationAngle
            );
        }

        //GUIUtils.DrawLineVertical(
        //    targetRect.xMax,
        //    targetRect.y,
        //    targetRect.height,
        //    Table.columnSeparatorLineColor
        //);

        TooltipHandler.TipRegion(targetRect, new TipSignal(description));

        Widgets.DrawHighlightIfMouseover(targetRect);

        return Widgets.ButtonInvisible(targetRect);
    }
    public ICell GetCellFor(RowType row)
    {
        if (!cellsCache.ContainsKey(row))
        {
            cellsCache[row] = CreateCell(row);
        }

        return cellsCache[row];
    }

    protected abstract ICell CreateCell(RowType row);
}

public class StatDefColumn(
    StatDef statDef
) : Column<ThingAlike>(
    statDef.defName,
    statDef.LabelCap,
    statDef.description
)
{
    protected override ICell CreateCell(ThingAlike thing)
    {
        var statReq = StatRequest.For(thing.def, thing.stuff);

        if (statDef.Worker.ShouldShowFor(statReq) == false)
        {
            return NumCell.Empty;
        }

        float valueAbs = float.NaN;
        string valueString = "";
        //string? valueExplanation = null;

        // Maybe add some indication that there was an exception.
        try
        {
            valueAbs = statDef.Worker.GetValue(statReq);
        }
        catch
        {
        }

        if (!float.IsNaN(valueAbs))
        {
            try
            {
                valueString = statDef.Worker.GetStatDrawEntryLabel(
                    statDef,
                    valueAbs,
                    ToStringNumberSense.Undefined,
                    statReq
                );
            }
            catch
            {
            }

            //try
            //{
            //    // Maybe we don't really need to cache explanation.
            //    // Because we only show one at a time.
            //    // The only issue is that it is shown at 60fps.
            //    valueExplanation = statDef.Worker.GetExplanationFull(statReq, ToStringNumberSense.Undefined, _valueAbs);
            //}
            //catch
            //{
            //}

            return new NumCell(
                valueAbs,
                string.IsNullOrEmpty(valueString) ? valueAbs + "" : valueString
            );
        }

        return NumCell.Empty;
    }
}

public class LabelColumn() : Column<ThingAlike>("Label", "Name", minWidth: 250f)
{
    protected override ICell CreateCell(ThingAlike thing)
    {
        return new StrCell(thing.label);
    }
}

public class WeaponRangeColumn() : Column<ThingAlike>(
    "WeaponRange",
    "Range".Translate(),
    "Stat_Thing_Weapon_Range_Desc".Translate()
)
{
    protected override ICell CreateCell(ThingAlike thing)
    {
        if (
            thing.def.IsRangedWeapon
            && thing.def.Verbs.Count > 0
        )
        {
            var value = thing.def.Verbs.First(v => v.isPrimary)?.range;

            if (value is float _value)
            {
                return new NumCell(_value, _value + "");
            }
        }

        return NumCell.Empty;
    }
}