using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Stats;

static class Columns
{
    static public readonly Dictionary<string, Column> list = [];
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

public abstract class Column(
    string id,
    string? label = null,
    string? description = null,
    float? minWidth = null
)
{
    public readonly string id = id;
    public readonly string label = label ?? "";
    public readonly string description = description ?? "";
    public readonly float minWidth = minWidth ?? 100f;

    public bool Draw(Rect targetRect, SortDirection? sortDirection = null)
    {
        //Widgets.DrawHighlight(targetRect);
        Cell.Label(targetRect, label);

        if (sortDirection != null)
        {
            var rotationAngle = sortDirection == SortDirection.Ascending ? -90f : 90f;

            Widgets.DrawTextureRotated(
                targetRect.RightPartPixels(Table.headersRowHeight),
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

    public abstract void DrawCellFor(Rect targetRect, FakeThing thing);
    public abstract void SortRows(List<FakeThing> thingDefs, SortDirection direction);
}

class StatDefColumn_CacheEntry(
    float valueAbs,
    string? valueString = null
)
{
    public readonly float valueAbs = valueAbs;
    public readonly string valueString = valueString ?? "";
}

public class StatDefColumn(
    StatDef statDef,
    string? label = null,
    float? minWidth = null
) : Column(
    statDef.defName,
    label ?? statDef.LabelCap,
    statDef.description,
    minWidth
)
{
    private readonly StatDef statDef = statDef;
    // Initialize in constructor with FakeThings.list.Count?
    private readonly Dictionary<FakeThing, StatDefColumn_CacheEntry?> cache = [];

    private StatDefColumn_CacheEntry? TryGetValueFor(FakeThing thing)
    {
        if (cache.ContainsKey(thing))
        {
            return cache[thing];
        }

        var statReq = StatRequest.For(thing.thingDef, thing.stuffDef);

        if (statDef.Worker.ShouldShowFor(statReq) == false)
        {
            return cache[thing] = null;
        }

        float? valueAbs = null;
        string? valueString = null;
        //string? valueExplanation = null;

        // Maybe add some indication that there was an exception.
        try
        {
            valueAbs = statDef.Worker.GetValue(statReq);
        }
        catch
        {
        }

        if (valueAbs is float _valueAbs)
        {
            try
            {
                valueString = statDef.Worker.GetStatDrawEntryLabel(statDef, _valueAbs, ToStringNumberSense.Undefined, statReq);
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

            return cache[thing] = new StatDefColumn_CacheEntry(_valueAbs, valueString);
        }

        return cache[thing] = null;
    }

    public override void DrawCellFor(Rect targetRect, FakeThing thing)
    {
        var cellValue = TryGetValueFor(thing);

        if (cellValue != null)
        {
            Cell.Label(targetRect, cellValue.valueString);
        }
    }
    public override void SortRows(List<FakeThing> thingDefs, SortDirection direction)
    {
        // Something is wrong with sorting.
        thingDefs.Sort((r1, r2) =>
        {
            var val1 = TryGetValueFor(r1)?.valueAbs;
            var val2 = TryGetValueFor(r2)?.valueAbs;

            if (val1 == val2)
            {
                return 0;
            }
            else if (val1 == null || val2 == null)
            {
                return -1;
            }
            else if (direction == SortDirection.Ascending)
            {
                return val1 > val2 ? 1 : -1;
            }
            else
            {
                return val1 < val2 ? 1 : -1;
            }
        });
    }
}

public class LabelColumn() : Column("Label", "Name", minWidth: 250f)
{
    public override void DrawCellFor(Rect targetRect, FakeThing thing)
    {
        //Cell.LabelWithDefIcon(targetRect, thing.icon, thing.label);
        //thing.label + "\n\n" + thing.thingDef.description
        Cell.LabelWithDefIcon(targetRect, thing, thing.label);
        Cell.Tip(targetRect, thing.thingDef.description);
        Cell.DefDialogOnClick(targetRect, thing);
    }
    public override void SortRows(List<FakeThing> thingDefs, SortDirection direction)
    {
        if (direction == SortDirection.Ascending)
        {
            thingDefs.Sort((r1, r2) =>
            {
                var val1 = r1.label;
                var val2 = r2.label;

                return val1.CompareTo(val2);
            });
        }
        else
        {
            thingDefs.Sort((r1, r2) =>
            {
                var val1 = r1.label;
                var val2 = r2.label;

                return val2.CompareTo(val1);
            });
        }
    }
}

// It is basically StatDefColumn
public class WeaponRangeColumn() : Column(
    "WeaponRange",
    "Range".Translate(),
    "Stat_Thing_Weapon_Range_Desc".Translate()
)
{
    private readonly Dictionary<FakeThing, StatDefColumn_CacheEntry?> cache = [];

    private StatDefColumn_CacheEntry? TryGetValueFor(FakeThing thing)
    {
        if (cache.ContainsKey(thing))
        {
            return cache[thing];
        }

        if (thing.thingDef.Verbs.Count == 0)
        {
            return cache[thing] = null;
        }

        var value = thing.thingDef.Verbs.First(v => v.isPrimary)?.range;

        if (value is float _value)
        {
            return cache[thing] = new StatDefColumn_CacheEntry(_value, value + "");
        }

        return cache[thing] = null;
    }

    public override void DrawCellFor(Rect targetRect, FakeThing thing)
    {
        var cellValue = TryGetValueFor(thing);

        if (cellValue != null)
        {
            Cell.Label(targetRect, cellValue.valueString);
        }
    }
    public override void SortRows(List<FakeThing> thingDefs, SortDirection direction) { }
}