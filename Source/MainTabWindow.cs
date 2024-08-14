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
    private readonly Table table;
    private Rect? preCloseRect = null;
    private Rect? preExpandRect = null;
    private bool isExpanded => preExpandRect != null;
    private Rect expandRect => new Rect(
        0f,
        0f,
        UI.screenWidth,
        windowRect.height = UI.screenHeight - MainButtonDef.ButtonHeight
    );

    private const float titleBarHeight = 30f;

    public static readonly Color borderLineColor = new(1f, 1f, 1f, 0.4f);

    public StatsMainTabWindow()
    {
        draggable = true;
        resizeable = true;

        table = new(Columns.list.Values.ToList(), FakeThings.list);

        Log.Message(Columns.list.Count);
        Log.Message(FakeThings.list.Count);

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
                        columnId => Columns.list[columnId]
                    ).ToList(),
                    FakeThings.list.Where(
                        ft => catDef.childThingDefs.Contains(ft.thingDef)
                    ).ToList()
                );
            }
        }
    }
    private void DrawTitleBar(Rect targetRect)
    {
        var labelRect = targetRect.LeftPartPixels(targetRect.width - titleBarHeight * 4);
        var labelText = categoryPicker.selectedCatDef?.LabelCap ?? "All";
        var currX = labelRect.xMax;
        var helpIconRect = new Rect(currX, 0f, titleBarHeight, titleBarHeight);
        currX += titleBarHeight;
        var minimizeButtonRect = new Rect(currX, 0f, titleBarHeight, titleBarHeight);
        currX += titleBarHeight;
        var expandButtonRect = new Rect(currX, 0f, titleBarHeight, titleBarHeight);
        currX += titleBarHeight;
        var closeButtonRect = new Rect(currX, 0f, titleBarHeight, titleBarHeight);
        using (new GUIUtils.TextAnchorContext(TextAnchor.MiddleLeft))
        {
            Widgets.DrawLightHighlight(targetRect);
            Widgets.Label(labelRect.ContractedBy(GenUI.Pad, 0f), labelText);
            Widgets.ButtonImage(helpIconRect, TexButton.Info, GUI.color, tooltip: "How to use:");

            if (Widgets.ButtonImageFitted(minimizeButtonRect, TexButton.Minus))
            {
                Minimize();
            }

            if (TitleBar.ButtonExpand(expandButtonRect))
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

            if (Widgets.ButtonImageFitted(closeButtonRect, TexButton.CloseXSmall))
            {
                Close();
            }
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
        using (new GUIUtils.TextWordWrapContext(false))
        {
            var titleBarRect = new Rect(0f, 0f, targetRect.width, titleBarHeight);
            var categoryPickerTargetRect = new Rect(
                0f,
                titleBarRect.yMax,
                300f,
                targetRect.height - titleBarRect.height
            );
            var tableRect = new Rect(
                categoryPickerTargetRect.xMax,
                titleBarRect.yMax,
                targetRect.width - categoryPickerTargetRect.width,
                targetRect.height - titleBarRect.height
            );

            DrawTitleBar(titleBarRect);
            Widgets.DrawLineHorizontal(
                0f,
                titleBarRect.yMax,
                targetRect.width,
                borderLineColor
            );
            categoryPicker.Draw(categoryPickerTargetRect, HandleCategoryChange);

            if (categoryPicker.selectedCatDef is ThingCategoryDef selCatDef)
            {
                tablesCache.TryGetValue(selCatDef.defName, out Table table);
                table?.Draw(tableRect);
            }
            else
            {
                table.Draw(tableRect);
            }
        }
    }
}

static class TitleBar
{
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