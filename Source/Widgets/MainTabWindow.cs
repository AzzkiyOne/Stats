using RimWorld;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

public sealed class MainTabWindow : RimWorld.MainTabWindow
{
    protected override float Margin { get => 1f; }
    public override Vector2 RequestedTabSize => new(UI.screenWidth, base.RequestedTabSize.y);
    private Rect? PreCloseRect = null;
    private Rect? PreExpandRect = null;
    private bool IsExpanded => PreExpandRect != null;
    private Rect ExpandRect => new(
        0f,
        0f,
        UI.screenWidth,
        windowRect.height = UI.screenHeight - MainButtonDef.ButtonHeight
    );
    internal const float TitleBarHeight = 30f;
    internal static readonly Color BorderLineColor = new(1f, 1f, 1f, 0.4f);
    private readonly Widget TitleBarWidget;
    private readonly MainTabWindowTitleBar TitleBar;
    private readonly TableSelector TableSelector;
    private TableDef _CurTableDef;
    private TableDef CurTableDef
    {
        set
        {
            if (_CurTableDef == value)
            {
                return;
            }

            _CurTableDef = value;
            TableSelector.TableDef = value;
            TitleBar.TableWidget = value.Widget;
        }
    }
    public MainTabWindow()
    {
        //draggable = true;
        //resizeable = true;

        // TODO: All of this TableDef/ITableWidget juggling,
        // can probably be replaced with a single stream of TableDefs.
        _CurTableDef = DefDatabase<TableDef>.GetNamed("RangedWeapons_ThingTable");
        TableSelector = new TableSelector(_CurTableDef);
        TableSelector.OnTableSelect += tableDef => CurTableDef = tableDef;
        TitleBar = new MainTabWindowTitleBar(
            _CurTableDef.Widget,
            TableSelector,
            //ResetWindow,
            ExpandOrCollapseWidow,
            //() => Close(),
            ResetCurrentTableFilters
        );
        TitleBarWidget = TitleBar.WidthRel(1f);
    }
    public override void DoWindowContents(Rect rect)
    {
        Text.WordWrap = false;

        TitleBarWidget.DrawIn(rect.TopPartPixels(TitleBarHeight));

        _CurTableDef.Widget.Draw(rect.BottomPartPixels(rect.height - TitleBarHeight));

        GUIDebugger.DrawDebugInfo(rect);

        Text.WordWrap = true;
    }
    private void ExpandOrCollapseWidow()
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
        //draggable = false;
        //resizeable = false;
        PreExpandRect = windowRect;
        windowRect = ExpandRect;
    }
    private void Collapse()
    {
        //draggable = true;
        //resizeable = true;

        if (PreExpandRect is Rect _preExpandRect)
        {
            windowRect = _preExpandRect;
            PreExpandRect = null;
        }
    }
    //private void ResetWindow()
    //{
    //    draggable = true;
    //    resizeable = true;
    //    PreExpandRect = null;
    //    SetInitialSizeAndPosition();
    //}
    public override void PreOpen()
    {
        base.PreOpen();

        if (IsExpanded)
        {
            windowRect = ExpandRect;
        }
        else if (PreCloseRect is Rect preCloseRect)
        {
            windowRect = preCloseRect;
        }
    }
    public override void PostClose()
    {
        base.PostClose();
        PreCloseRect = windowRect;
    }
    private void ResetCurrentTableFilters()
    {
        _CurTableDef.Widget.ResetFilters();
    }
}
