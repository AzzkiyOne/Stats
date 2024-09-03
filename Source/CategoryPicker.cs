using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats;

class CategoryPicker
{
    private const float rowHeight = 22f;
    private const float labelPadding = 5f;
    private const float indentSize = 20f;
    private Vector2 scrollPosition;
    private readonly DynamicThingCategoryDef rootCategory;
    public DynamicThingCategoryDef selectedCategory;
    private int debug_rowsDrawn = 0;
    private int totalRowsDisplayed = 0;
    private readonly List<DynamicThingCategoryDef> openedCategories = [];
    public CategoryPicker()
    {
        selectedCategory = rootCategory = DynamicThingCategoryDefOf.Root;
    }
    public void Draw(Rect targetRect, Action<DynamicThingCategoryDef> onCategoryChange)
    {
        using (new TextAnchorCtx(TextAnchor.MiddleLeft))
        {
            // Scroll area size correction only works because of how rows are "culled" at the top.
            // We don't render them, but still counting.
            var displayedRowsHeight = rowHeight * totalRowsDisplayed;
            var contentRect = new Rect(
                0f,
                0f,
                targetRect.width,
                displayedRowsHeight >= targetRect.height
                    ? displayedRowsHeight + targetRect.height
                    : displayedRowsHeight
            );
            var currY = 0f;

            Widgets.BeginScrollView(targetRect, ref scrollPosition, contentRect);

            debug_rowsDrawn = 0;
            totalRowsDisplayed = 0;

            foreach (var catDef in rootCategory.Children)
            {
                DrawRows(targetRect, ref currY, catDef, onCategoryChange);
            }

            Widgets.EndScrollView();

            Debug.TryDrawUIDebugInfo(targetRect, debug_rowsDrawn + "");
        }
    }
    private void DrawRows(
        Rect parentRect,
        ref float currY,
        DynamicThingCategoryDef catDef, Action<DynamicThingCategoryDef> onCategoryChange,
        float treeLevel = 0
    )
    {
        if (catDef.Items.Count == 0 && !Debug.InDebugMode)
        {
            return;
        }

        totalRowsDisplayed++;

        var indentAmount = treeLevel * indentSize;
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
                catDef.Children.Count != 0
                && Widgets.ButtonImage(
                    collapseControlRect,
                    openedCategories.Contains(catDef) ? TexButton.Collapse : TexButton.Reveal
                )
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
            var labelRect = contentRect
                .RightPartPixels(contentRect.width - iconRect.width)
                .ContractedBy(labelPadding, 0);
            string labelText = Debug.InDebugMode ? catDef.defName : catDef.LabelCap;

            Widgets.DrawTextureFitted(iconRect, catDef.Icon, 0.9f);
            Widgets.Label(labelRect, labelText);

            Widgets.DrawHighlightIfMouseover(contentRect);

            if (Widgets.ButtonInvisible(contentRect))
            {
                if (catDef == selectedCategory)
                {
                    selectedCategory = rootCategory;
                }
                else
                {
                    selectedCategory = catDef;
                }

                onCategoryChange(selectedCategory);
            }

            if (selectedCategory == catDef)
            {
                Widgets.DrawHighlight(contentRect);
            }

            debug_rowsDrawn++;
        }

        currY += rowHeight;

        if (openedCategories.Contains(catDef))
        {
            foreach (var childCat in catDef.Children)
            {
                DrawRows(parentRect, ref currY, childCat, onCategoryChange, treeLevel + 1);
            }
        }
    }
}
