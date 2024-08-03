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

enum TablePreset
{
    All,
    WeaponsRanged,
    WeaponsMelee,
    Apparel,
    Stuff,
    Building,
    Building2,
    Plant,
    Misc,// Meals/Drugs etc
    Animal,// Maybe?
}

public class StatsMainTabWindow : MainTabWindow
{
    Dictionary<TablePreset, Table> tables = [];
    TablePreset currentTable = TablePreset.WeaponsRanged;
    override protected float Margin { get => 1f; }
    public StatsMainTabWindow()
    {
        draggable = true;
        resizeable = true;

        // All
        tables.Add(
            TablePreset.All,
            new Table(
                [new LabelColumn()],
                DefDatabase<ThingDef>.AllDefs.Select(def => new Row(def)).ToList()
            )
        );

        // Ranged weapons
        List<Column> rwtColumns = [
            new LabelColumn(),
            new StatColumn(
                key: "Damage",
                label: "Damage",
                description: "Stat_Thing_Damage_Desc".Translate()
            ),
            new StatColumn(
                key: "Armor penetration",
                label: "AP",
                description: "ArmorPenetrationExplanation".Translate()
            ),
            new StatColumn(
                key: "Range",
                label: "Range",
                description: "Stat_Thing_Weapon_Range_Desc".Translate()
            ),
            new StatColumn(
                key: "Aiming time",
                label: "AT",
                description: "Stat_Thing_Weapon_RangedWarmupTime_Desc".Translate()
            ),
            new StatColumn(
                key: "Ranged cooldown",
                label: "RCD",
                description: DefDatabase<StatDef>.GetNamed("RangedWeapon_Cooldown").description
            ),
            new StatColumn(
                key: "Stopping power",
                label: "SP",
                description: "StoppingPowerExplanation".Translate()
            ),
        ];
        // Bunch of columns for testing
        for (int i = 0; i < 10; i++)
        {
            rwtColumns.Add(new StatColumn(
                key: "Stopping power",
                label: "Test",
                description: "Test"
            ));
        }
        tables.Add(
            TablePreset.WeaponsRanged,
            new Table(
                rwtColumns,
                //DefDatabase<ThingDef>.AllDefs.Where(def => def.IsRangedWeapon && def.AllRecipes.Count > 0).ToList()
                DefDatabase<ThingDef>.AllDefs.Where(def => def.IsRangedWeapon).Select(def => new Row(def)).ToList()
            )
        );

        // Melee weapons
        tables.Add(
            TablePreset.WeaponsMelee,
            new Table(
                [new LabelColumn()],
                DefDatabase<ThingDef>.AllDefs.Where(def => def.IsMeleeWeapon).Select(def => new Row(def)).ToList()
            )
        );

        // Apparel
        tables.Add(
            TablePreset.Apparel,
            new Table(
                [new LabelColumn()],
                DefDatabase<ThingDef>.AllDefs.Where(def => def.IsApparel).Select(def => new Row(def)).ToList()
            )
        );

        // Stuff
        tables.Add(
            TablePreset.Stuff,
            new Table(
                [
                    new LabelColumn(),
                    new StatColumn(
                        key: "Insulation - Cold",
                        label: "IC",
                        description: "Insulation - Cold"
                    ),
                    new StatColumn(
                        key: "Insulation - Heat",
                        label: "IH",
                        description: "Insulation - Heat"
                    ),
                    new StatColumn(
                        key: "Max hit points",
                        label: "Max HP",
                        description: "Max hit points"
                    ),
                ],
                DefDatabase<ThingDef>.AllDefs.Where(def => def.IsStuff).Select(def => new Row(def)).ToList()
            )
        );

        // Buildings
        tables.Add(
            TablePreset.Building,
            new Table(
                [new LabelColumn()],
                DefDatabase<ThingDef>.AllDefs.Where(def => def.IsBuildingArtificial).Select(def => new Row(def)).ToList()
            )
        );

        // Buildings 2
        tables.Add(
            TablePreset.Building2,
            new Table(
                [new LabelColumn()],
                DefDatabase<ThingDef>.AllDefs.Where(def => def.IsWithinCategory(DefDatabase<ThingCategoryDef>.GetNamed("Buildings"))).Select(def => new Row(def)).ToList()
            )
        );

        // Plants
        tables.Add(
            TablePreset.Plant,
            new Table(
                [new LabelColumn()],
                DefDatabase<ThingDef>.AllDefs.Where(def => def.IsPlant).Select(def => new Row(def)).ToList()
            )
        );
    }
    public override void DoWindowContents(Rect rect)
    {
        var buttonsRect = new Rect(0f, 0f, 50f, rect.height);
        var tableRect = new Rect(buttonsRect.xMax, 0f, rect.width - buttonsRect.width, rect.height);

        tables.TryGetValue(currentTable, out Table table);

        table?.Draw(tableRect);

        Widgets.BeginGroup(buttonsRect);

        Vector2 iconSize = new(30f, 30f);
        float buttonSize = buttonsRect.width;
        float currY = 0f;

        //if (Widgets.ButtonImageWithBG(new Rect(0f, currY, buttonSize, buttonSize), DefDatabase<ThingDef>.GetNamed("Gun_AssaultRifle").uiIcon, iconSize))
        //{
        //    currentTable = TablePreset.WeaponsRanged;
        //}
        if (Widgets.ButtonText(new Rect(0f, currY, buttonSize, buttonSize), "All"))
        {
            currentTable = TablePreset.All;
        }
        currY += buttonSize;
        if (
            currentTable == TablePreset.WeaponsRanged
            ? Widgets.ButtonImage(new Rect(0f, currY, buttonSize, buttonSize), Widgets.ButtonBGAtlasClick)
            : Widgets.ButtonImageWithBG(new Rect(0f, currY, buttonSize, buttonSize), DefDatabase<ThingDef>.GetNamed("Gun_AssaultRifle").uiIcon, iconSize)
        )
        {
            currentTable = TablePreset.WeaponsRanged;
        }
        currY += buttonSize;
        if (Widgets.ButtonImageWithBG(new Rect(0f, currY, buttonSize, buttonSize), DefDatabase<ThingDef>.GetNamed("MeleeWeapon_BreachAxe").uiIcon, iconSize))
        {
            currentTable = TablePreset.WeaponsMelee;
        }
        currY += buttonSize;
        if (Widgets.ButtonImageWithBG(new Rect(0f, currY, buttonSize, buttonSize), DefDatabase<ThingDef>.GetNamed("Apparel_Duster").uiIcon, iconSize))
        {
            currentTable = TablePreset.Apparel;
        }
        currY += buttonSize;
        if (Widgets.ButtonImageWithBG(new Rect(0f, currY, buttonSize, buttonSize), DefDatabase<ThingDef>.GetNamed("Cloth").uiIcon, iconSize))
        {
            currentTable = TablePreset.Stuff;
        }
        currY += buttonSize;
        if (Widgets.ButtonImageWithBG(new Rect(0f, currY, buttonSize, buttonSize), DefDatabase<ThingDef>.GetNamed("Bed").uiIcon, iconSize))
        {
            currentTable = TablePreset.Building;
        }
        currY += buttonSize;
        if (Widgets.ButtonImageWithBG(new Rect(0f, currY, buttonSize, buttonSize), DefDatabase<ThingDef>.GetNamed("DiningChair").uiIcon, iconSize))
        {
            currentTable = TablePreset.Building2;
        }
        currY += buttonSize;
        if (Widgets.ButtonImageWithBG(new Rect(0f, currY, buttonSize, buttonSize), DefDatabase<ThingDef>.GetNamed("Plant_Rice").uiIcon, iconSize))
        {
            currentTable = TablePreset.Plant;
        }

        Widgets.EndGroup();

        //if (Widgets.ButtonText(new Rect(0f, 0f, 250f, Table.rowHeight), currentTable))
        //{
        //    Find.WindowStack.Add(new FloatMenu([
        //        new FloatMenuOption("Weapons (R)", () => currentTable = "Weapons (R)"),
        //        new FloatMenuOption("Stuff", () => currentTable = "Stuff"),
        //    ]));
        //}
    }
}
