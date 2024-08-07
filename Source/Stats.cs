﻿using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Stats;

[StaticConstructorOnStartup]
public static class Stats
{
    static Stats()
    {
    }
}

static class ColumnSetDB
{
    static private readonly Dictionary<string, ColumnSet> columnSets = new() { ["Root"] = new ColumnSet() };
    static public ColumnSet? GetColumnSetForCatDef(ThingCategoryDef catDef)
    {
        columnSets.TryGetValue(catDef.defName, out ColumnSet? columnSet);

        if (columnSet == null)
        {
            foreach (var parentCatDef in catDef.Parents)
            {
                return GetColumnSetForCatDef(parentCatDef);
            }
        }

        return columnSet;
    }
    static public void Add(ColumnSet columnSet)
    {
        foreach (string category in columnSet.categories)
        {
            columnSets.TryAdd(category, columnSet);
        }
    }
}

class ColumnSet
{
    public List<string> categories = ["Root"];
    public List<ColumnDef> columns = [
        new LabelColumnDef(),
        new StatColumnDef("MaxHitPoints")
        {
            label = "HP",
        },
        new StatColumnDef("MarketValue")
        {
            label = "$",
        },
        new StatColumnDef("Mass"),
        new StatColumnDef("Bulk"),// CE
    ];
}

public class StatsMainTabWindow : MainTabWindow
{
    protected override float Margin { get => 1f; }
    private readonly CategoryPicker categoryPicker;
    private readonly Dictionary<string, Table> tablesCache = [];
    public StatsMainTabWindow()
    {
        draggable = true;
        resizeable = true;

        ColumnSetDB.Add(new ColumnSet
        {
            categories = ["WeaponsRanged"],
            columns = [
                new LabelColumnDef(),
                new RangeColumnDef(),
                new StatColumnDef("Recoil"),// CE
                new StatColumnDef("SwayFactor"),// CE
                new StatColumnDef("ShotSpread"),// CE
                new StatColumnDef("SightsEfficiency"),// CE
                new StatColumnDef("MagazineCapacity")// CE
                {
                    drawRawValue = true,
                },
                new StatColumnDef("ReloadTime")// CE
                {
                    drawRawValue = true,
                },
                new StatColumnDef("TicksBetweenBurstShots"),// CE
                new StatColumnDef("Caliber")// CE
                {
                    isSortable = false,
                },
                new StatColumnDef("OneHandedness"),// CE
                new StatColumnDef("Mass"),
                new StatColumnDef("Bulk"),// CE
            ],
        });

        categoryPicker = new CategoryPicker();
        HandleCategoryChange(categoryPicker.selectedCatDef);
    }
    private void HandleCategoryChange(ThingCategoryDef? catDef)
    {
        if (catDef != null && !tablesCache.ContainsKey(catDef.defName))
        {
            var columnSet = ColumnSetDB.GetColumnSetForCatDef(catDef);

            if (columnSet != null)
            {
                tablesCache[catDef.defName] = new Table(
                    columnSet.columns,
                    catDef.childThingDefs.Select(thingDef => new Row(thingDef)).ToList()
                );
            }
        }
    }
    public override void DoWindowContents(Rect targetRect)
    {
        var categoryPickerTargetRect = new Rect(0f, 0f, 300f, targetRect.height);
        categoryPicker.Draw(categoryPickerTargetRect, HandleCategoryChange);

        var tableRect = new Rect(categoryPickerTargetRect.xMax, 0f, targetRect.width - categoryPickerTargetRect.width, targetRect.height);
        if (categoryPicker.selectedCatDef is ThingCategoryDef selCatDef)
        {
            tablesCache.TryGetValue(selCatDef.defName, out Table table);
            table?.Draw(tableRect);
        }
    }
}

class CategoryPicker
{
    private const float rowHeight = 22f;
    private const float labelPadding = 5f;
    private const float indentSize = 20f;
    private Vector2 scrollPosition;
    private readonly ThingCategoryDef rootCatDef;
    public ThingCategoryDef? selectedCatDef;
    private int debug_rowsDrawn = 0;
    private int totalRowsDisplayed = 0;
    private readonly List<ThingCategoryDef> openedCategories = [];
    public CategoryPicker()
    {
        rootCatDef = DefDatabase<ThingCategoryDef>.GetNamed("Root");
    }
    public void Draw(Rect targetRect, Action<ThingCategoryDef> onCategoryChange)
    {
        using (new GUIUtils.GameFontContext(GameFont.Tiny))
        using (new GUIUtils.TextAnchorContext(TextAnchor.MiddleLeft))
        {
            // Scroll area size correction only works because of how rows are "culled" at the top.
            // We don't render them, but still counting.
            var displayedRowsHeight = rowHeight * totalRowsDisplayed;
            var contentRect = new Rect(0f, 0f, targetRect.width, displayedRowsHeight >= targetRect.height ? displayedRowsHeight + targetRect.height : displayedRowsHeight);

            Widgets.BeginScrollView(targetRect, ref scrollPosition, contentRect);

            debug_rowsDrawn = 0;
            totalRowsDisplayed = 0;

            var currY = 0f;
            foreach (var catDef in rootCatDef.childCategories)
            {
                DrawRows(targetRect, ref currY, catDef, onCategoryChange);
            }

            Widgets.EndScrollView();

            Debug.TryDrawUIDebugInfo(targetRect, debug_rowsDrawn + "");
        }
    }
    private void DrawRows(Rect parentRect, ref float currY, ThingCategoryDef catDef, Action<ThingCategoryDef> onCategoryChange)
    {
        if (catDef.childThingDefs.Count == 0 && catDef.childCategories.Count == 0 && !Debug.InDebugMode())
        {
            return;
        }

        totalRowsDisplayed++;

        // "-1" because we don't draw the root category entry.
        var indentAmount = (catDef.Parents.Count() - 1) * indentSize;
        var rowRect = new Rect(
            indentAmount,
            currY,
            parentRect.width - indentAmount,
            rowHeight
        );

        // Culling
        if (rowRect.y - scrollPosition.y > parentRect.height)
        {
            return;
        }
        if (rowRect.yMax - scrollPosition.y > 0)
        {
            var collapseControlRect = rowRect.LeftPartPixels(rowHeight);
            var contentRect = rowRect.RightPartPixels(rowRect.width - collapseControlRect.width);
            if (
                catDef.childCategories.Count != 0
                && Widgets.ButtonImage(collapseControlRect, openedCategories.Contains(catDef) ? TexButton.Collapse : TexButton.Reveal)
            )
            {
                if (openedCategories.Contains(catDef))
                {
                    openedCategories.Remove(catDef);
                }
                else
                {
                    openedCategories.Add(catDef);
                }
            }

            var iconRect = contentRect.LeftPartPixels(rowHeight);
            Widgets.DrawTextureFitted(iconRect, catDef.icon, 0.9f);
            var labelRect = contentRect.RightPartPixels(contentRect.width - iconRect.width).ContractedBy(labelPadding, 0);
            string labelText = Debug.InDebugMode() ? catDef.defName : catDef.LabelCap;
            if (string.IsNullOrEmpty(labelText))
            {
                if (!string.IsNullOrEmpty(catDef.label))
                {
                    labelText = catDef.label;
                }
                else
                {
                    labelText = catDef.defName;
                }
            }
            Widgets.LabelEllipses(labelRect, labelText);

            if (catDef.childThingDefs.Count > 0)
            {
                Widgets.DrawHighlightIfMouseover(contentRect);

                if (
                    Widgets.ButtonInvisible(contentRect)
                    && catDef != selectedCatDef
                )
                {
                    selectedCatDef = catDef;
                    onCategoryChange(catDef);
                }
            }

            if (selectedCatDef == catDef)
            {
                Widgets.DrawHighlight(contentRect);
            }

            debug_rowsDrawn++;
        }

        currY += rowHeight;

        if (openedCategories.Contains(catDef))
        {
            foreach (var childCat in catDef.childCategories)
            {
                DrawRows(parentRect, ref currY, childCat, onCategoryChange);
            }
        }
    }
}