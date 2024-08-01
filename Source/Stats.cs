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
    Table rangedWeaponsTable;
    Table stuffTable;
    //string currentTable = "Stuff";
    string currentTable = "Weapons (R)";
    public override void PostOpen()
    {
        draggable = true;
        resizeable = true;

        List<Column> rwtColumns = [
            new LabelColumn(label: "Label"),
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
            // Bunch of columns for testing
            new StatColumn(
                key: "Stopping power",
                label: "SP",
                description: "StoppingPowerExplanation".Translate()
            ),
            new StatColumn(
                key: "Stopping power",
                label: "SP",
                description: "StoppingPowerExplanation".Translate()
            ),
            new StatColumn(
                key: "Stopping power",
                label: "SP",
                description: "StoppingPowerExplanation".Translate()
            ),
            new StatColumn(
                key: "Stopping power",
                label: "SP",
                description: "StoppingPowerExplanation".Translate()
            ),
            new StatColumn(
                key: "Stopping power",
                label: "SP",
                description: "StoppingPowerExplanation".Translate()
            ),
            new StatColumn(
                key: "Stopping power",
                label: "SP",
                description: "StoppingPowerExplanation".Translate()
            ),
            new StatColumn(
                key: "Stopping power",
                label: "SP",
                description: "StoppingPowerExplanation".Translate()
            ),
        ];
        //DefDatabase<ThingDef>.AllDefs.Where(def => def.IsRangedWeapon && def.AllRecipes.Count > 0)
        var rwtRows = DefDatabase<ThingDef>.AllDefs.Where(def => def.IsRangedWeapon).Select(def => new Row(def)).ToList();
        rangedWeaponsTable = new Table(rwtColumns, rwtRows);

        List<Column> stColumns = [
            new LabelColumn(),
            new StatColumn(
                key: "Insulation - Cold",
                label: "IC"
            ),
            new StatColumn(
                key: "Insulation - Heat",
                label: "IH"
            ),
            new StatColumn(
                key: "Max hit points",
                label: "Max HP"
            ),
        ];
        var stRows = DefDatabase<ThingDef>.AllDefs.Where(def => def.IsStuff).Select(def => new Row(def)).ToList();
        stuffTable = new Table(stColumns, stRows);
    }
    public override void DoWindowContents(Rect rect)
    {
        switch (currentTable)
        {
            case "Weapons (R)":
                rangedWeaponsTable.Draw(rect);
                break;
            case "Stuff":
                stuffTable.Draw(rect);
                break;

        };

        //if (Widgets.ButtonText(new Rect(0f, 0f, 250f, Table.rowHeight), currentTable))
        //{
        //    Find.WindowStack.Add(new FloatMenu([
        //        new FloatMenuOption("Weapons (R)", () => currentTable = "Weapons (R)"),
        //        new FloatMenuOption("Stuff", () => currentTable = "Stuff"),
        //    ]));
        //}
    }
}
