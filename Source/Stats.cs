using System;
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

public class StatsMainTabWindow : MainTabWindow
{
    override protected float Margin { get => 1f; }
    private readonly CategoryPicker categoryPicker;
    private readonly Dictionary<string, Table> tablesCache = [];
    private readonly List<ColumnDef> columnDefs = [
        new LabelColumnDef([]),
        new StatColumnDef("MaxHitPoints", [], "HP"),
        new StatColumnDef("MarketValue", [], "$"),
        new StatColumnDef("Mass", []),
        new StatColumnDef("Bulk", []),
        new StatColumnDef("Caliber", []),
    ];
    public StatsMainTabWindow()
    {
        draggable = true;
        resizeable = true;

        categoryPicker = new CategoryPicker();
        HandleCategoryChange(categoryPicker.selectedCatDef);
    }
    private IEnumerable<Row> GetTableRows(ThingCategoryDef catDef)
    {
        // Same thing can be in the category and in its descendants simultaneously.

        // Self
        foreach (var childThingDef in catDef.SortedChildThingDefs)
        {
            yield return new Row(childThingDef);
        }

        // Subcategories
        foreach (var childCatDef in catDef.childCategories)
        {
            var rows = GetTableRows(childCatDef);

            foreach (var row in rows)
            {
                yield return row;
            }
        }
    }
    private void HandleCategoryChange(ThingCategoryDef catDef)
    {
        if (!tablesCache.ContainsKey(catDef.defName))
        {
            tablesCache[catDef.defName] = new Table(
                columnDefs,
                GetTableRows(catDef).ToList()
            );
        }
    }
    public override void DoWindowContents(Rect targetRect)
    {
        var categoryPickerTargetRect = new Rect(0f, 0f, 300f, targetRect.height);
        categoryPicker.Draw(categoryPickerTargetRect, HandleCategoryChange);

        var tableRect = new Rect(categoryPickerTargetRect.xMax, 0f, targetRect.width - categoryPickerTargetRect.width, targetRect.height);
        tablesCache.TryGetValue(categoryPicker.selectedCatDef.defName, out Table table);
        table?.Draw(tableRect);
    }
}

class CategoryPicker
{
    const float rowHeight = 22f;
    const float cellPadding = 5f;
    const float indentSize = 20f;
    Vector2 scrollPosition;
    readonly ThingCategoryDef rootCatDef;
    public ThingCategoryDef selectedCatDef;
    private readonly int categoriesCount;
    public CategoryPicker()
    {
        selectedCatDef = rootCatDef = DefDatabase<ThingCategoryDef>.GetNamed("Root");
        categoriesCount = DefDatabase<ThingCategoryDef>.AllDefs.Count();
    }
    public void Draw(Rect targetRect, Action<ThingCategoryDef> onCategoryChange)
    {
        var contentRect = new Rect(0f, 0f, targetRect.width, rowHeight * categoriesCount);

        Widgets.BeginScrollView(targetRect, ref scrollPosition, contentRect);

        TextAnchor prevTextAnchor = Text.Anchor;
        Text.Anchor = TextAnchor.MiddleLeft;

        var currY = 0f;
        DrawRows(targetRect.width, ref currY, rootCatDef, onCategoryChange);

        Text.Anchor = prevTextAnchor;

        Widgets.EndScrollView();
    }
    // TODO: Add culling.
    void DrawRows(float rowWidth, ref float currY, ThingCategoryDef catDef, Action<ThingCategoryDef> onCategoryChange)
    {
        var rowRect = new Rect(
            // Skipping root category indent. This is clearly a crutch.
            catDef.defName == "Root"
            ? 0f
            : (catDef.Parents.Count() - 1) * indentSize,
            currY,
            rowWidth,
            rowHeight
        );

        Text.Font = GameFont.Tiny;

        if (Widgets.ButtonInvisible(rowRect) && catDef != selectedCatDef)
        {
            selectedCatDef = catDef;
            onCategoryChange(catDef);
        }
        if (selectedCatDef == catDef)
        {
            Widgets.DrawHighlight(rowRect);
        }
        Widgets.DrawHighlightIfMouseover(rowRect);
        Widgets.DrawTextureFitted(rowRect.LeftPartPixels(rowHeight), catDef.icon, 0.9f);
        Widgets.LabelEllipses(rowRect.RightPartPixels(rowRect.width - rowHeight).ContractedBy(cellPadding, 0), catDef.defName);

        currY += rowHeight;

        foreach (var childCat in catDef.childCategories)
        {
            DrawRows(rowWidth, ref currY, childCat, onCategoryChange);
        }
    }
}