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
    const float labelPadding = 5f;
    const float indentSize = 20f;
    Vector2 scrollPosition;
    readonly ThingCategoryDef rootCatDef;
    public ThingCategoryDef selectedCatDef;
    private readonly int categoriesCount;
    private int debug_rowsDrawn = 0;
    private int totalRowsDisplayed = 0;
    private List<ThingCategoryDef> openedCategories = [];
    public CategoryPicker()
    {
        selectedCatDef = rootCatDef = DefDatabase<ThingCategoryDef>.GetNamed("Root");
        categoriesCount = DefDatabase<ThingCategoryDef>.AllDefs.Count();
        openedCategories.Add(rootCatDef);
    }
    public void Draw(Rect targetRect, Action<ThingCategoryDef> onCategoryChange)
    {
        // Scroll area size correction only works because of how rows are "culled" at the top.
        // We don't render them, but still counting.
        var displayedRowsHeight = rowHeight * totalRowsDisplayed;
        var contentRect = new Rect(0f, 0f, targetRect.width, displayedRowsHeight > targetRect.height ? displayedRowsHeight + targetRect.height : displayedRowsHeight);

        Widgets.BeginScrollView(targetRect, ref scrollPosition, contentRect);

        TextAnchor prevTextAnchor = Text.Anchor;
        Text.Anchor = TextAnchor.MiddleLeft;

        debug_rowsDrawn = 0;
        totalRowsDisplayed = 0;

        var currY = 0f;
        DrawRows(targetRect, ref currY, rootCatDef, onCategoryChange);

        Text.Anchor = prevTextAnchor;

        Widgets.EndScrollView();

        Widgets.Label(new Rect(targetRect.width / 2, targetRect.height / 2, 20f, 20f), debug_rowsDrawn + "");
    }
    private void DrawRows(Rect parentRect, ref float currY, ThingCategoryDef catDef, Action<ThingCategoryDef> onCategoryChange)
    {
        totalRowsDisplayed++;

        var indentAmount = (catDef.Parents.Count() - 1) * indentSize;
        var rowRect = new Rect(
            // Skipping root category indent. This is clearly a crutch.
            catDef.defName == "Root"
            ? 0f
            : indentAmount,
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
            Text.Font = GameFont.Tiny;

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
            Widgets.DrawHighlightIfMouseover(contentRect);
            var iconRect = contentRect.LeftPartPixels(rowHeight);
            Widgets.DrawTextureFitted(iconRect, catDef.icon, 0.9f);
            var labelRect = contentRect.RightPartPixels(contentRect.width - iconRect.width).ContractedBy(labelPadding, 0);
            Widgets.LabelEllipses(labelRect, catDef.defName);
            if (Widgets.ButtonInvisible(contentRect) && catDef != selectedCatDef)
            {
                selectedCatDef = catDef;
                onCategoryChange(catDef);
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