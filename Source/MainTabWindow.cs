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
    private readonly Dictionary<string, Table<ThingAlike>> tablesCache = [];
    private readonly Table<ThingAlike> table;
    private Rect? preCloseRect = null;
    private Rect? preExpandRect = null;
    private bool isExpanded => preExpandRect != null;
    private Rect expandRect => new(
        0f,
        0f,
        UI.screenWidth,
        windowRect.height = UI.screenHeight - MainButtonDef.ButtonHeight
    );

    private const float titleBarHeight = 30f;
    private const float catPickerWidth = 300f;

    public static readonly Color borderLineColor = new(1f, 1f, 1f, 0.4f);

    public StatsMainTabWindow()
    {
        draggable = true;
        resizeable = true;

        //table = new(Columns.list.Values.ToList(), FakeThings.list);
        table = new(
            Columns.list.Values.ToList(),
            ThingAlikes.list.Where(t =>
                (
                    t.def.thingCategories == null
                    || t.def.thingCategories.Count == 0
                )
                && t.def.designationCategory == null
            ).ToList()
        );

        Log.Message(Columns.list.Count);
        Log.Message(ThingAlikes.list.Count);

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
                tablesCache[catDef.defName] = new Table<ThingAlike>(
                    columnSet.columns.Select(
                        columnId => Columns.list[columnId]
                    ).ToList(),
                    ThingAlikes.list.Where(
                        ft => catDef.AllThingDefs().Contains(ft.def)
                    ).ToList()
                );
            }
        }
    }
    private void ExpandOrCollapse()
    {
        if (isExpanded)
        {
            Collapse();
        }
        else
        {
            Expand();
        }
    }
    private void Expand()
    {
        draggable = false;
        resizeable = false;

        preExpandRect = windowRect;
        windowRect = expandRect;
    }
    private void Collapse()
    {
        draggable = true;
        resizeable = true;

        if (preExpandRect is Rect _preExpandRect)
        {
            windowRect = _preExpandRect;
            preExpandRect = null;
        }
    }
    private void Minimize()
    {
        draggable = true;
        resizeable = true;
        preExpandRect = null;

        SetInitialSizeAndPosition();
    }

    public override void PreOpen()
    {
        base.PreOpen();

        if (isExpanded)
        {
            windowRect = expandRect;
        }
        else if (preCloseRect is Rect _preCloseRect)
        {
            windowRect = _preCloseRect;
        }
    }
    public override void PostClose()
    {
        base.PostClose();

        preCloseRect = windowRect;
    }
    public override void DoWindowContents(Rect targetRect)
    {
        var titleBarText = categoryPicker.selectedCatDef?.LabelCap ?? "All";
        var currY = targetRect.y;

        using (new Utils.GUI.TextWordWrapContext(false))
        {
            switch (TitleBar.Draw(
                targetRect.CutFromY(ref currY, titleBarHeight),
                titleBarText
            ))
            {
                case TitleBarEvent.Minimize:
                    Minimize();
                    break;
                case TitleBarEvent.Expand:
                    ExpandOrCollapse();
                    break;
                case TitleBarEvent.Close:
                    Close();
                    break;
            }

            DrawContent(targetRect.CutFromY(ref currY));
        }
    }
    private void DrawContent(Rect targetRect)
    {
        var currX = targetRect.x;

        categoryPicker.Draw(
            targetRect.CutFromX(ref currX, catPickerWidth),
            HandleCategoryChange
        );

        if (categoryPicker.selectedCatDef is ThingCategoryDef selCatDef)
        {
            tablesCache.TryGetValue(selCatDef.defName, out Table<ThingAlike> table);
            table?.Draw(targetRect.CutFromX(ref currX));
        }
        else
        {
            table.Draw(targetRect.CutFromX(ref currX));
        }
    }
}

enum TitleBarEvent
{
    Minimize,
    Expand,
    Close,
}

static class TitleBar
{
    public static TitleBarEvent? Draw(Rect targetRect, string text)
    {
        var buttonWidth = targetRect.height;
        var labelWidth = targetRect.width - buttonWidth * 4;
        var currX = targetRect.x;
        TitleBarEvent? Event = null;

        using (new Utils.GUI.TextAnchorContext(TextAnchor.MiddleLeft))
        {
            Widgets.DrawLightHighlight(targetRect);

            Widgets.DrawLineHorizontal(
                targetRect.x,
                targetRect.yMax,
                targetRect.width,
                StatsMainTabWindow.borderLineColor
            );

            Widgets.Label(
                targetRect
                    .CutFromX(ref currX, labelWidth)
                    .ContractedBy(GenUI.Pad, 0f),
                text
            );

            Widgets.ButtonImage(
                targetRect.CutFromX(ref currX, buttonWidth),
                TexButton.Info,
                GUI.color,
                tooltip: "How to use:"
            );

            if (Widgets.ButtonImageFitted(
                targetRect.CutFromX(ref currX, buttonWidth),
                TexButton.Reveal
            ))
            {
                Event = TitleBarEvent.Minimize;
            }

            if (ButtonExpand(targetRect.CutFromX(ref currX, buttonWidth)))
            {

                Event = TitleBarEvent.Expand;
            }

            if (Widgets.ButtonImageFitted(
                targetRect.CutFromX(ref currX, buttonWidth),
                TexButton.CloseXSmall
            ))
            {
                Event = TitleBarEvent.Close;
            }
        }

        return Event;
    }
    public static bool ButtonExpand(Rect targetRect)
    {
        GUI.color = (Mouse.IsOver(targetRect) ? GenUI.MouseoverColor : Color.white);

        Widgets.DrawTextureRotated(targetRect, TexButton.ShowZones, 90f);

        GUI.color = Color.white;

        TooltipHandler.TipRegion(targetRect, "Maximize/restore window");

        var isClicked = Widgets.ButtonInvisible(targetRect);

        GUI.color = Color.white;

        return isClicked;
    }
}