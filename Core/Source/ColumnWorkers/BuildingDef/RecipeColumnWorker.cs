//using System;
//using System.Collections.Generic;
//using System.Linq;
//using RimWorld;
//using Stats.Widgets;
//using UnityEngine;
//using Verse;

//namespace Stats;

//public sealed class Building_RecipeColumnWorker : ColumnWorker<ThingAlike>
//{
//    public Building_RecipeColumnWorker(ColumnDef columnDef) : base(columnDef, ColumnCellStyle.String)
//    {
//    }
//    private static readonly Func<ThingAlike, Dictionary<ThingDef, int>?> GetCostList =
//    FunctionExtensions.Memoized((ThingAlike thing) =>
//    {
//        if (thing.Def.BuildableByPlayer == false)
//        {
//            return null;
//        }

//        var thingDefCostList = thing.Def.CostList;
//        var costList = new Dictionary<ThingDef, int>();

//        if (thingDefCostList?.Count > 0)
//        {
//            foreach (var thingDefCount in thingDefCostList)
//            {
//                costList[thingDefCount.thingDef] = thingDefCount.count;
//            }
//        }

//        if (thing.StuffDef != null && thing.Def.CostStuffCount > 0)
//        {
//            var costListEntryExists = costList.ContainsKey(thing.StuffDef);
//            var stuffCost = thing.StuffDef.smallVolume
//                ? thing.Def.CostStuffCount * ThingDef.SmallUnitPerVolume
//                : thing.Def.CostStuffCount;

//            if (costListEntryExists)
//            {
//                costList[thing.StuffDef] += stuffCost;
//            }
//            else
//            {
//                costList.Add(thing.StuffDef, stuffCost);
//            }
//        }

//        return costList;
//    });
//    // TODO: This is mostly a copy paste from Thing_RecipesColumnWorker.
//    public override Widget? GetTableCellWidget(ThingAlike thing)
//    {
//        var costList = GetCostList(thing);

//        if (costList == null || costList.Count == 0)
//        {
//            return null;
//        }

//        // Ingredients
//        var ingredientWidgets = new List<Widget>(costList.Keys.Count);

//        foreach (var (thingDef, cost) in costList.OrderBy(entry => entry.Key.GetStatValueAbstract(StatDefOf.MarketValue)))
//        {
//            var ingredientWidget = new HorizontalContainer([
//                new Label(costList[thingDef].ToString()),
//                new ThingIcon(thingDef)
//                    .ToButtonGhostly(() => Draw.DefInfoDialog(thingDef))
//                    .Tooltip(thingDef.LabelCap),
//            ], 2f);

//            ingredientWidgets.Add(ingredientWidget);
//        }

//        // Work amount
//        var workAmount = thing.Def.GetStatValuePerceived(StatDefOf.WorkToBuild, thing.StuffDef);

//        // Skill(s)
//        // TODO: Handle artistic skill prerequisite (if we even need to).
//        var constructionSkillLevelString = thing.Def.constructionSkillPrerequisite > 0
//            ? thing.Def.constructionSkillPrerequisite.ToString().Colorize(Globals.GUI.TextColorHighlight)
//            : "any";

//        return new VerticalContainer([
//            new HorizontalContainer(ingredientWidgets, Globals.GUI.PadSm)
//                .PaddingAbs(0f, 0f, 0f, Globals.GUI.PadXs),
//            new HorizontalContainer([
//                new Label($"<i>{Mathf.CeilToInt(workAmount / 60f)}</i>")
//                    .Tooltip($"<i>{StatDefOf.WorkToBuild.LabelCap}</i>"),
//                new Label($" | {SkillDefOf.Construction.LabelCap}: {constructionSkillLevelString}"),
//            ])
//        ]);
//    }
//    private static readonly Func<ThingAlike, HashSet<ThingDef>> GetResourceDefs =
//    FunctionExtensions.Memoized((ThingAlike thing) =>
//    {
//        return GetCostList(thing)?.Keys.ToHashSet() ?? [];
//    });
//    public override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<ThingAlike> tableRecords)
//    {
//        yield return new(new Label("Has any"), Make.BooleanFilter<ThingAlike>(thing => GetCostList(thing) != null));
//        yield return new(new Label("Resources"), Make.MTMThingDefFilter(GetResourceDefs, tableRecords));
//        yield return new(new Label(SkillDefOf.Construction.LabelCap), Make.NumberFilter<ThingAlike>(thing => thing.Def.constructionSkillPrerequisite));
//    }
//    private static readonly Func<ThingAlike, float> GetWorkAmount =
//    FunctionExtensions.Memoized((ThingAlike thing) =>
//    {
//        return thing.Def.GetStatValuePerceived(StatDefOf.WorkToBuild, thing.StuffDef);
//    });
//    public override int Compare(ThingAlike thing1, ThingAlike thing2)
//    {
//        return GetWorkAmount(thing1).CompareTo(GetWorkAmount(thing2));
//    }
//}
