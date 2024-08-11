using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Stats;

public class StatsMainTabWindow : MainTabWindow
{
    protected override float Margin { get => 1f; }
    private readonly CategoryPicker categoryPicker;
    private readonly Dictionary<string, Table> tablesCache = [];
    public StatsMainTabWindow()
    {
        draggable = true;
        resizeable = true;

        Log.Message(ThingDefTable.columns.Count);
        Log.Message(ThingDefTable.rows.Count);

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
                    columnSet.columns.Select(
                        columnId => ThingDefTable.columns[columnId]
                    ).ToList(),
                    catDef.childThingDefs.Select(
                        thingDef => ThingDefTable.rows[thingDef]
                    ).ToList()
                );
            }
        }
    }
    public override void DoWindowContents(Rect targetRect)
    {
        using (new GUIUtils.TextWordWrapContext(false))
        {
            var categoryPickerTargetRect = new Rect(0f, 0f, 300f, targetRect.height);
            var tableRect = new Rect(
                categoryPickerTargetRect.xMax,
                0f,
                targetRect.width - categoryPickerTargetRect.width,
                targetRect.height
            );

            categoryPicker.Draw(categoryPickerTargetRect, HandleCategoryChange);

            if (categoryPicker.selectedCatDef is ThingCategoryDef selCatDef)
            {
                tablesCache.TryGetValue(selCatDef.defName, out Table table);
                table?.Draw(tableRect);
            }
        }
    }
}
