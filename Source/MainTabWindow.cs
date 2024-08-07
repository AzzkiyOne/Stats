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

        ColumnSetDB.Add(new ColumnSet
        {
            categories = ["WeaponsRanged"],
            columns = [
                new LabelColumn(),
                new WeaponRangeColumn(),
                new StatColumn("Recoil"),// CE
                new StatColumn("SwayFactor"),// CE
                new StatColumn("ShotSpread"),// CE
                new StatColumn("SightsEfficiency"),// CE
                new StatColumn("MagazineCapacity")// CE
                {
                    drawRawValue = true,
                },
                new StatColumn("ReloadTime")// CE
                {
                    drawRawValue = true,
                },
                new StatColumn("TicksBetweenBurstShots"),// CE
                new StatColumn("Caliber")// CE
                {
                    isSortable = false,
                },
                new StatColumn("OneHandedness"),// CE
                new StatColumn("Mass"),
                new StatColumn("Bulk"),// CE
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
