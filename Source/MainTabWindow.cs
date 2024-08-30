using System;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Stats;

public class StatsMainTabWindow : MainTabWindow
{
    protected override float Margin { get => 1f; }
    public override Vector2 RequestedTabSize => new(UI.screenWidth, base.RequestedTabSize.y);

    private readonly CategoryPicker categoryPicker;
    private GenTable<ColumnDef, ThingAlike> thingDefsTable;
    private Rect? preCloseRect = null;
    private Rect? preExpandRect = null;
    private bool IsExpanded => preExpandRect != null;
    private Rect ExpandRect => new(
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

        // Adjust column widths.
        //
        // Why here of all the places?
        //
        // Calling Text.CalcSize before GUI has been initialized will cause a crash.
        // So i can't call it in Def.PostLoad/ResolveReferences where it would make
        // more sense.
        foreach (var columnDef in DefDatabase<ColumnDef>.AllDefs)
        {
            columnDef.minWidth = Math.Max(
                Text.CalcSize(columnDef.label).x + 15f,
                columnDef.minWidth
            );
        }

        thingDefsTable = new(
            DefDatabase<ColumnDef>.AllDefsListForReading,
            //ThingAlikes.list.Where(t =>
            //    (
            //        t.def.thingCategories == null
            //        || t.def.thingCategories.Count == 0
            //    )
            //    && t.def.designationCategory == null
            //).ToList()
            ThingAlikes.list
        );

        Log.Message(DefDatabase<ColumnDef>.AllDefsListForReading.Count);
        Log.Message(ThingAlikes.list.Count);

        categoryPicker = new CategoryPicker();
        HandleCategoryChange(categoryPicker.selectedCatDef);
    }

    private void DrawContent(Rect targetRect)
    {
        var currX = targetRect.x;

        categoryPicker.Draw(
            targetRect.CutFromX(ref currX, catPickerWidth),
            HandleCategoryChange
        );

        thingDefsTable.Draw(targetRect.CutFromX(ref currX));
    }
    private void HandleCategoryChange(ThingCategoryDef? catDef)
    {
        if (catDef != null)
        {
            thingDefsTable.Rows = catDef.AllThingAlikes().ToList();

            var columnSet = ColumnSetDB.GetColumnSetForCatDef(catDef);

            if (columnSet != null)
            {
                thingDefsTable.Columns = columnSet.columns.Select(
                    columnId => DefDatabase<ColumnDef>.GetNamed(columnId)
                ).ToList();
            }
        }
        else
        {
            thingDefsTable.Columns = DefDatabase<ColumnDef>.AllDefsListForReading;
            thingDefsTable.Rows = ThingAlikes.list;
        }
    }
    private void ExpandOrCollapse()
    {
        if (IsExpanded)
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
        windowRect = ExpandRect;
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

        if (IsExpanded)
        {
            windowRect = ExpandRect;
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
        var titleBarText = "Things";
        var currY = targetRect.y;

        if (thingDefsTable.SelectedRow?.label is not null)
        {
            titleBarText += " / " + thingDefsTable.SelectedRow.label;
        }

        using (new TextWordWrapCtx(false))
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

        using (new TextAnchorCtx(TextAnchor.MiddleLeft))
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
                tooltip: "How to use:"
            );

            if (GUIWidgets.ButtonImage(
                targetRect.CutFromX(ref currX, buttonWidth),
                TexButton.Reveal,
                angle: 90f
            ))
            {
                Event = TitleBarEvent.Minimize;
            }

            if (GUIWidgets.ButtonImage(
                targetRect.CutFromX(ref currX, buttonWidth),
                TexButton.ShowZones,
                "Maximize/restore window",
                90f
            ))
            {

                Event = TitleBarEvent.Expand;
            }

            if (Widgets.ButtonImage(
                targetRect.CutFromX(ref currX, buttonWidth),
                TexButton.CloseXSmall
            ))
            {
                Event = TitleBarEvent.Close;
            }
        }

        return Event;
    }
}