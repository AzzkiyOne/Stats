//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using RimWorld;
//using Stats.Widgets;
//using UnityEngine;
//using Verse;

//namespace Stats;

//[StaticConstructorOnStartup]
//public sealed class Thing_RecipesColumnWorker : ColumnWorker<ThingAlike>
//{
//    private static readonly Texture2D IngredientTexture;
//    public Thing_RecipesColumnWorker(ColumnDef columnDef) : base(columnDef, ColumnCellStyle.String)
//    {
//    }
//    static Thing_RecipesColumnWorker()
//    {
//        IngredientTexture = ContentFinder<Texture2D>.Get("UI/Icons/ThingCategories/ResourcesRaw");
//    }
//    // TODO: Handle same single ThingDef ingredients.
//    private static IEnumerable<(ThingDef[] ThingDefs, int Count)> GetThingDefRecipeIngredients(RecipeDef recipeDef, ThingDef? stuffDef)
//    {
//        foreach (var ingredient in recipeDef.ingredients)
//        {
//            var ingredientAllowedThingDefsCount = ingredient.filter.AllowedDefCount;

//            if (ingredientAllowedThingDefsCount == 1)
//            {
//                var ingredientThingDef = ingredient.filter.AnyAllowedDef;
//                var ingredientCount = ingredient.CountRequiredOfFor(ingredientThingDef, recipeDef);

//                yield return ([ingredientThingDef], ingredientCount);
//            }
//            else if (ingredientAllowedThingDefsCount > 1)
//            {
//                if (stuffDef != null && ingredient.filter.Allows(stuffDef))
//                {
//                    var ingredientThingDef = stuffDef;
//                    var ingredientCount = ingredient.CountRequiredOfFor(stuffDef, recipeDef);

//                    yield return ([ingredientThingDef], ingredientCount);
//                }
//                else
//                {
//                    yield return (ingredient.filter.AllowedThingDefs.ToArray(), (int)ingredient.GetBaseCount());
//                }
//            }
//        }
//    }
//    public override Widget? GetTableCellWidget(ThingAlike thing)
//    {
//        var recipeDefs = thing.Def.GetRecipeDefs();

//        if (recipeDefs == null || recipeDefs.Count == 0)
//        {
//            return null;
//        }

//        var recipeWidgets = new List<Widget>();

//        foreach (var recipeDef in recipeDefs)
//        {
//            Widget recipeWidget = RecipeDefToWidget(recipeDef, thing).WidthRel(1f);

//            if (recipeWidgets.Count != 0)
//            {
//                recipeWidget = recipeWidget
//                    .PaddingAbs(0f, 0f, Globals.GUI.PadSm, 0f)
//                    .BorderTop(Widgets.MainTabWindow.BorderLineColor)
//                    .PaddingAbs(0f, 0f, Globals.GUI.PadSm, 0f);
//            }

//            recipeWidgets.Add(recipeWidget);
//        }

//        return new VerticalContainer(recipeWidgets);
//    }
//    private static Widget RecipeDefToWidget(RecipeDef recipeDef, ThingAlike thing)
//    {
//        var rows = new List<Widget>();

//        // Ingredients
//        if (recipeDef.ingredients.Count > 0)
//        {
//            var ingredients = GetThingDefRecipeIngredients(recipeDef, thing.StuffDef)
//                .OrderBy(ingredient => ingredient.ThingDefs[0].GetStatValueAbstract(StatDefOf.MarketValue));
//            var ingredientWidgets = new List<Widget>(recipeDef.ingredients.Count);

//            foreach (var (thingDefs, count) in ingredients)
//            {
//                Widget icon;

//                if (thingDefs.Length == 1)
//                {
//                    var thingDef = thingDefs[0];

//                    icon = new ThingIcon(thingDef)
//                        .ToButtonGhostly(() => Draw.DefInfoDialog(thingDef))
//                        .Tooltip(thingDef.LabelCap);
//                }
//                else
//                {
//                    var stringBuilder = new StringBuilder();

//                    foreach (var thingDef in thingDefs)
//                    {
//                        var line = $"- {thingDef.LabelCap}";

//                        if (thingDef.smallVolume)
//                        {
//                            line += $" x{ThingDef.SmallUnitPerVolume}".Colorize(Globals.GUI.TextColorHighlight);
//                        }

//                        stringBuilder.AppendLine(line);
//                    }

//                    icon = new Icon(IngredientTexture).Tooltip(stringBuilder.ToString());
//                }

//                var ingredientWidget = new HorizontalContainer([new Label(count.ToString()), icon], 2f);

//                ingredientWidgets.Add(ingredientWidget);
//            }

//            Widget row = new HorizontalContainer(ingredientWidgets, Globals.GUI.PadSm);

//            // Product amount
//            var productAmount = 0;

//            foreach (var product in recipeDef.products)
//            {
//                if (product.thingDef == thing.Def)
//                {
//                    productAmount = product.count;
//                    break;
//                }
//            }

//            if (productAmount > 1)
//            {
//                row = new HorizontalContainer([new Label("("), row, new Label($") / {productAmount}")]);
//            }

//            rows.Add(row.PaddingAbs(0f, 0f, 0f, Globals.GUI.PadXs));
//        }

//        // Recipe user(s), work amount and primary skill(s)
//        var recipeUsers = recipeDef.GetAllRecipeUsers();

//        if (recipeUsers?.Count > 0)
//        {
//            // Work amount
//            // TODO: Find where the 60 is defined.
//            var workAmount = Mathf.CeilToInt(recipeDef.WorkAmountForStuff(thing.StuffDef) / 60f);
//            var workAmountTooltip = $"<i>{StatDefOf.WorkToMake.LabelCap}</i>";
//            var workAmountWidget = new Label($"<i>{workAmount}</i> | ").Tooltip(workAmountTooltip);

//            if (recipeDef.workSpeedStat != null)
//            {
//                // TODO: Add more factors. Importantly, stuff-related ones.
//                workAmountTooltip += $"\n\nFactors:\n- {recipeDef.workSpeedStat.LabelCap}";
//            }

//            // Recipe users
//            var cheapestRecipeUser = recipeUsers.OrderBy(thingDef => thingDef.GetStatValueAbstract(StatDefOf.MarketValue)).First();
//            var recipeUsersWidget = new ThingIcon(cheapestRecipeUser)
//                .ToButtonGhostly(() => Draw.DefInfoDialog(cheapestRecipeUser))
//                .Tooltip(cheapestRecipeUser.LabelCap);

//            if (recipeUsers.Count > 1)
//            {
//                var allRecipeUsers = recipeUsers
//                    .OrderBy(thingDef => thingDef.label)
//                    .Select(thingDef => $"- {thingDef.LabelCap}");
//                var recipeUsersTooltip = string.Join("\n", allRecipeUsers);
//                recipeUsersWidget = new HorizontalContainer([
//                    recipeUsersWidget,
//                    new Label($" ({recipeUsers.Count})".Colorize(Globals.GUI.TextColorSecondary))
//                ]).Tooltip(recipeUsersTooltip);
//            }

//            // Skill(s)
//            Widget? skillsWidget = null;

//            if (recipeDef.workSkill != null)
//            {
//                var workSkillLevel = 0;
//                var skillsCount = 1;
//                string? tooltip = null;
//                var stringBuilder = new StringBuilder();

//                if (recipeDef.skillRequirements?.Count > 0)
//                {
//                    foreach (var skillReq in recipeDef.skillRequirements)
//                    {
//                        if (skillReq.skill != recipeDef.workSkill)
//                        {
//                            stringBuilder.AppendInNewLine(MakeSkillReqLine(skillReq.skill, skillReq.minLevel));
//                            skillsCount++;
//                        }
//                        else
//                        {
//                            workSkillLevel = skillReq.minLevel;
//                        }
//                    }
//                }

//                var skillLevelString = workSkillLevel > 0
//                    ? workSkillLevel.ToString().Colorize(Globals.GUI.TextColorHighlight)
//                    : SkillLevelToString(workSkillLevel);
//                var skillString = $" | {recipeDef.workSkill.LabelCap}: {skillLevelString}";

//                if (skillsCount > 1)
//                {
//                    skillString += $" ({skillsCount})".Colorize(Globals.GUI.TextColorSecondary);
//                    tooltip = MakeSkillReqLine(recipeDef.workSkill, workSkillLevel, true) + "\n" + stringBuilder.ToString();
//                }

//                skillsWidget = new Label(skillString);

//                if (tooltip?.Length > 0)
//                {
//                    skillsWidget = skillsWidget.Tooltip(tooltip);
//                }
//            }

//            var rowWidgets = new List<Widget>() { workAmountWidget, recipeUsersWidget };

//            if (skillsWidget != null)
//            {
//                rowWidgets.Add(skillsWidget);
//            }

//            var row = new HorizontalContainer(rowWidgets);

//            rows.Add(row);
//        }

//        return new VerticalContainer(rows);
//    }
//    private static string MakeSkillReqLine(SkillDef skillDef, int level, bool isWorkSkill = false)
//    {
//        var skillLabel = skillDef.LabelCap;

//        if (isWorkSkill)
//        {
//            skillLabel = $"<i>{skillLabel}</i>";
//        }

//        return $"- {skillLabel}: {SkillLevelToString(level)}";
//    }
//    private static string SkillLevelToString(int level)
//    {
//        return level == 0 ? "any" : level.ToString();
//    }
//    private static readonly Func<ThingAlike, HashSet<ThingDef>> GetAllRecipesUsers =
//    FunctionExtensions.Memoized((ThingAlike thing) =>
//    {
//        var allRecipesUsers = new HashSet<ThingDef>();
//        var recipeDefs = thing.Def.GetRecipeDefs();

//        if (recipeDefs?.Count > 0)
//        {
//            foreach (var recipeDef in recipeDefs)
//            {
//                var recipeUsers = recipeDef.GetAllRecipeUsers();

//                if (recipeUsers != null)
//                {
//                    allRecipesUsers.AddRange(recipeUsers);
//                }
//            }
//        }

//        return allRecipesUsers;
//    });
//    private static readonly Func<ThingAlike, decimal> GetWorkSkillLevel =
//    FunctionExtensions.Memoized((ThingAlike thing) =>
//    {
//        var recipeDefs = thing.Def.GetRecipeDefs();

//        if (recipeDefs?.Count > 0)
//        {
//            foreach (var recipeDef in recipeDefs)
//            {
//                if (recipeDef.skillRequirements?.Count > 0)
//                {
//                    foreach (var skillReq in recipeDef.skillRequirements)
//                    {
//                        if (skillReq.skill == recipeDef.workSkill)
//                        {
//                            return skillReq.minLevel;
//                        }
//                    }
//                }
//            }
//        }

//        return 0m;
//    });
//    private static readonly Func<ThingAlike, HashSet<ThingDef>> GetAllIngredients =
//    FunctionExtensions.Memoized((ThingAlike thing) =>
//    {
//        var allIngredients = new HashSet<ThingDef>();
//        var recipeDefs = thing.Def.GetRecipeDefs();

//        if (recipeDefs?.Count > 0)
//        {
//            foreach (var recipeDef in recipeDefs)
//            {
//                var ingredients = GetThingDefRecipeIngredients(recipeDef, thing.StuffDef);

//                allIngredients.AddRange(ingredients.SelectMany((ingredient) => ingredient.ThingDefs));
//            }
//        }

//        return allIngredients;
//    });
//    public override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<ThingAlike> tableRecords)
//    {
//        yield return new(new Label("Has any"), Make.BooleanFilter<ThingAlike>(thing => thing.Def.GetRecipeDefs() != null));
//        yield return new(new Label("Resources"), Make.MTMThingDefFilter(GetAllIngredients, tableRecords));
//        yield return new(new Label("Bench"), Make.MTMThingDefFilter(GetAllRecipesUsers, tableRecords));
//        yield return new(new Label("Work skill"), Make.NumberFilter(GetWorkSkillLevel));
//    }
//    private static readonly Func<ThingAlike, float> GetMinWorkAmount =
//    FunctionExtensions.Memoized((ThingAlike thing) =>
//    {
//        var recipeDefs = thing.Def.GetRecipeDefs();

//        if (recipeDefs?.Count > 0)
//        {
//            return recipeDefs.Select(recipeDef => recipeDef.WorkAmountForStuff(thing.StuffDef)).Min();
//        }

//        return 0f;
//    });
//    public override int Compare(ThingAlike thing1, ThingAlike thing2)
//    {
//        return GetMinWorkAmount(thing1).CompareTo(GetMinWorkAmount(thing2));
//    }
//}
