using System.Collections.Generic;
using RimWorld;
using Stats.Utils;
using Stats.Utils.Extensions;
using Stats.Utils.GUIScopes;
using UnityEngine;
using Verse;
using static Stats.GUIStyles.MainTabWindow;

namespace Stats;

public sealed partial class MainTabWindow : RimWorld.MainTabWindow
{
    public override Vector2 RequestedTabSize => new(UI.screenWidth, windowRect.height);

    protected override float Margin { get => 1f; }
    private readonly List<MainTabWindowTab> _tabs;
    private MainTabWindowTab? _activeTab;
    private readonly FloatMenu _tabDefsMenu;
    private static readonly TipSignal _openTabButtonTooltip = "Open tab";
    private Vector2 _tabListScrollPosition;
    private readonly float _defaultHeight;
    private bool _isResized;
    private float _resizeYOffset;
    private float _yMax;

    public MainTabWindow()
    {
        _defaultHeight = base.RequestedTabSize.y;
        windowRect.height = _defaultHeight;
        List<TabDef> tabDefs = DefDatabase<TabDef>.AllDefsListForReading;
        int tabDefsCount = tabDefs.Count;
        List<FloatMenuOption> tabDefsMenuOptions = new(tabDefsCount);
        for (int i = 0; i < tabDefsCount; i++)
        {
            TabDef tabDef = tabDefs[i];
            FloatMenuOption menuOption = new(
                tabDef.LabelCap,
                () => OpenTab(tabDef),
                tabDef.Icon,
                tabDef.iconColor
            );
            tabDefsMenuOptions.Add(menuOption);
        }
        tabDefsMenuOptions.SortBy(option => option.Label);

        _tabDefsMenu = new FloatMenu(tabDefsMenuOptions);
        _tabs = new(tabDefsCount);
    }

    public override void DoWindowContents(Rect rect)
    {
        Event @event = Event.current;

        // TODO: Remove this after you'll explixitly set word wrap for every inner widget.
        bool wordWrap = Text.WordWrap;
        Text.WordWrap = false;

        // Layout
        rect
            .CutLeft(out Rect toolbarRect, ToolbarWidth)
            .TakeRest(out Rect tableRect);
        toolbarRect
            .CutTop(out Rect openTabButtonRect, ToolbarWidth)
            .TakeRest(out Rect tabListRect);
        tableRect.CutTop(out Rect expandButtonRect, GUIStyles.TableToolbar.Height);

        // Border
        if (@event.type == EventType.Repaint)
        {
            toolbarRect.DrawBorderRight(BorderColor);
        }

        // Buttons
        DrawOpenTabButton(openTabButtonRect);

        // Tab list
        // TODO:
        // - Add culling.
        // - Add reordering.
        Rect tabListContentRect = new(0f, 0f, ToolbarWidth, _tabs.Count * ToolbarWidth);
        using (new GUIScrollScope(tabListRect, ref _tabListScrollPosition, tabListContentRect, false))
        {
            Rect tabButtonRect = tabListContentRect with { height = ToolbarWidth };
            int tabsCount = _tabs.Count;
            for (int i = 0; i < tabsCount; i++)
            {
                MainTabWindowTab tab = _tabs[i];
                tab.DrawTabTitle(tabButtonRect, _activeTab == tab);
                tabButtonRect.y = tabButtonRect.yMax;
            }
        }

        // Tab
        _activeTab?.Draw(tableRect);

        Text.WordWrap = wordWrap;

        DoResizeControl(expandButtonRect);
    }

    private void DrawOpenTabButton(Rect rect)
    {
        if (Event.current.type == EventType.Repaint)
        {
            rect
                .HighlightLight()
                .DrawBorderBottom(BorderColor)
                .ContractedBy(IconPadding)
                .DrawTextureFitted(TexButton.Plus)
                .Tip(_openTabButtonTooltip);
        }

        if (rect.ButtonGhostly())
        {
            _tabDefsMenu.Open();
        }
    }

    private void DoResizeControl(Rect rect)
    {
        Event @event = Event.current;

        if (@event is { type: EventType.MouseDown, button: 0, modifiers: EventModifiers.None } && Mouse.IsOver(rect))
        {
            if (@event.clickCount > 1)
            {
                ResetSize();
            }
            else
            {
                _isResized = true;
                _resizeYOffset = @event.mousePosition.y;
            }
        }
        else if (_isResized)
        {
            if (OriginalEventUtility.EventType == EventType.MouseDrag)
            {
                float y = UI.GUIToScreenPoint(@event.mousePosition).y - _resizeYOffset;
                windowRect.yMin = Mathf.Clamp(y, 0f, _yMax);
                @event.Use();
            }
            else if (@event.rawType == EventType.MouseUp)
            {
                _isResized = false;
                GUIUtils.ReleaseMouseControl();
                @event.Use();
            }
        }

        GUI.Button(rect, GUIContent.none, GUIStyle.none);
    }

    private void OpenTab(TabDef tabDef)
    {
        MainTabWindowTab tab = tabDef.MakeTab();
        _tabs.Insert(0, tab);
        SelectTab(tab);
        tab.OnClick += SelectTab;
        tab.OnClose += CloseTab;
    }

    private void CloseTab(MainTabWindowTab tab)
    {
        int tabIndex = _tabs.IndexOf(tab);
        bool tabIsSelected = _activeTab == tab;

        if (tabIsSelected)
        {
            if (tabIndex > 0)
            {
                SelectTab(_tabs[tabIndex - 1]);
            }
            else if (_tabs.Count > 1)
            {
                SelectTab(_tabs[tabIndex + 1]);
            }
            else
            {
                _activeTab = null;
            }
        }

        _tabs.RemoveAt(tabIndex);
        tab.Dispose();
    }

    private void SelectTab(MainTabWindowTab tab)
    {
        _activeTab?.Unfocus();
        _activeTab = tab;
        tab.Focus();
    }

    private void ResetSize()
    {
        windowRect.height = _defaultHeight;
        SetInitialSizeAndPosition();
    }

    public override void PostOpen()
    {
        _yMax = UI.screenHeight - MainButtonDef.ButtonHeight - GUIStyles.TableToolbar.Height;
        base.PostOpen();
        _activeTab?.Focus();
    }

    public override void PostClose()
    {
        _isResized = false;
        _activeTab?.Unfocus();

        base.PostClose();
    }
}
