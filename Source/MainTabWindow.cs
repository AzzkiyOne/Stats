using RimWorld;
using UnityEngine;
using Verse;

namespace Stats;

public class StatsMainTabWindow : MainTabWindow
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
    private const float TitleBarHeight = 30f;
    internal static readonly Color BorderLineColor = new(1f, 1f, 1f, 0.4f);
    private readonly Widget_WindowTitleBar TitleBar;
    private readonly Widget_TableSelector TableSelector;
    public StatsMainTabWindow()
    {
        draggable = true;
        resizeable = true;
        TableSelector = new();
        TitleBar = new(TableSelector);
    }
    public override void DoWindowContents(Rect targetRect)
    {
        Text.WordWrap = false;

        switch (TitleBar.Draw(targetRect.CutByY(TitleBarHeight)))
        {
            case WindowTitleBarWidgetEvent.Minimize:
                Minimize();
                break;
            case WindowTitleBarWidgetEvent.Expand:
                ExpandOrCollapse();
                break;
            case WindowTitleBarWidgetEvent.Close:
                Close();
                break;
        }

        TableSelector.CurTableDef.Widget.Draw(targetRect);

        Text.WordWrap = true;
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
        PreExpandRect = windowRect;
        windowRect = ExpandRect;
    }
    private void Collapse()
    {
        draggable = true;
        resizeable = true;

        if (PreExpandRect is Rect _preExpandRect)
        {
            windowRect = _preExpandRect;
            PreExpandRect = null;
        }
    }
    private void Minimize()
    {
        draggable = true;
        resizeable = true;
        PreExpandRect = null;
        SetInitialSizeAndPosition();
    }
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
}
