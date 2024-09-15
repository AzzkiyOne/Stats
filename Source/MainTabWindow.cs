using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Stats;

public class StatsMainTabWindow : MainTabWindow
{
    protected override float Margin { get => 1f; }
    public override Vector2 RequestedTabSize => new(UI.screenWidth, base.RequestedTabSize.y);

    private ThingDefTable.Table thingDefsTable;
    private GeneDefTable.Table geneDefsTable;
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
    private Filters<ThingDefTable.ThingAlike> Filters { get; }
    public StatsMainTabWindow()
    {
        draggable = true;
        resizeable = true;
        Filters = new((filters) =>
        {
            thingDefsTable.Rows = ThingDefTable.ThingAlike.All
            .Where(thing => filters.All(filter => filter.Match(thing)))
            .Select(thing => new GenTable.Row<ThingDefTable.ThingAlike>(thing, DefDatabase<GenTable.ColumnDef>.AllDefsListForReading.Count))
            .ToList();
        });
        var tdfColumns = new List<GenTable.IColumn<ThingDefTable.ThingAlike>>();

        foreach (var column in DefDatabase<GenTable.ColumnDef>.AllDefs)
        {
            if (column is GenTable.IColumn<ThingDefTable.ThingAlike> _column)
            {
                tdfColumns.Add(_column);
            }
        }

        thingDefsTable = new(tdfColumns, ThingDefTable.ThingAlike.All);

        var gdtColumns = new List<GenTable.IColumn<GeneDef>>();

        foreach (var column in DefDatabase<GenTable.ColumnDef>.AllDefs)
        {
            if (column is GenTable.IColumn<GeneDef> _column)
            {
                gdtColumns.Add(_column);
            }
        }

        geneDefsTable = new(gdtColumns, DefDatabase<GeneDef>.AllDefsListForReading);
    }

    private void DrawContent(Rect targetRect)
    {
        var currX = targetRect.x;
        var filtersRect = targetRect.CutFromX(ref currX, 300f);
        Filters.Draw(filtersRect);

        thingDefsTable.Draw(targetRect.CutFromX(ref currX));
        //geneDefsTable.Draw(targetRect.CutFromX(ref currX));
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