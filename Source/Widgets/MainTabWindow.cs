using UnityEngine;
using Verse;

namespace Stats.Widgets;

public sealed class MainTabWindow : RimWorld.MainTabWindow
{
    protected override float Margin { get => 1f; }
    public override Vector2 RequestedTabSize => new(UI.screenWidth, base.RequestedTabSize.y);
    private bool IsExpanded;
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
            TitleBar.TableWidget = value.Worker.TableWidget;
        }
    }
    public MainTabWindow()
    {
        // TODO: All of this TableDef/ITableWidget juggling,
        // can probably be replaced with a single stream of TableDefs.
        _CurTableDef = TableDefOf.RangedWeapons_ThingTable;
        TableSelector = new TableSelector(_CurTableDef);
        TableSelector.OnTableSelect += tableDef => CurTableDef = tableDef;
        TitleBar = new MainTabWindowTitleBar(
            _CurTableDef.Worker.TableWidget,
            TableSelector,
            ExpandOrResetWidow,
            ResetCurrentTableFilters
        );
        TitleBarWidget = TitleBar.WidthRel(1f);
    }
    public override void DoWindowContents(Rect rect)
    {
        Text.WordWrap = false;

        TitleBarWidget.DrawIn(rect.TopPartPixels(TitleBarHeight));

        _CurTableDef.Worker.TableWidget.Draw(rect.BottomPartPixels(rect.height - TitleBarHeight));

        GUIDebugger.DrawDebugInfo(rect);

        Text.WordWrap = true;
    }
    private void ExpandOrResetWidow()
    {
        if (IsExpanded)
        {
            SetInitialSizeAndPosition();
        }
        else
        {
            windowRect.yMin = 0f;
        }

        IsExpanded = !IsExpanded;
    }
    public override void PreOpen()
    {
        base.PreOpen();

        if (IsExpanded)
        {
            windowRect.yMin = 0f;
        }
    }
    private void ResetCurrentTableFilters()
    {
        _CurTableDef.Worker.TableWidget.ResetFilters();
    }
}
