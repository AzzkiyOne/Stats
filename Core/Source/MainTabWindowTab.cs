using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stats.Utils;
using Stats.Utils.Extensions;
using UnityEngine;
using Verse;

namespace Stats;

public abstract class MainTabWindowTab
{
    internal event Action<MainTabWindowTab>? OnClick;
    internal event Action<MainTabWindowTab>? OnClose;

    private readonly TipSignal _tooltip;
    private readonly Texture2D _icon;
    private readonly Color _iconColor;
    private readonly float _iconScale;
    private readonly FloatMenu _menu;

    protected MainTabWindowTab(TabDef def)
    {
        _tooltip = def.LabelCap;
        if (def.description?.Length > 0)
        {
            _tooltip += $"\n\n{def.description}";
        }
        _icon = def.Icon;
        _iconColor = def.iconColor;
        _iconScale = def.iconScale;
        List<FloatMenuOption> menuOptions = [
            new FloatMenuOption("Close", () => OnClose?.Invoke(this))
        ];
        _menu = new FloatMenu(menuOptions);
    }

    internal void DrawTabTitle(Rect rect, DragManager<MainTabWindowTab> dragManager, bool isSelected)
    {
        Event @event = Event.current;

        if (@event.type == EventType.Repaint)
        {
            if (dragManager.IsDragged(this))
            {
                rect.HighlightDragged();
            }
            else if (isSelected)
            {
                rect.HighlightSelected();
            }

            rect.Tip(_tooltip)
                .ContractedBy(GUIStyles.MainTabWindow.IconPadding)
                .DrawTextureFitted(_icon, _iconColor, _iconScale);
        }

        if (@event is { type: EventType.MouseUp, modifiers: EventModifiers.None } && Mouse.IsOver(rect))
        {
            if (@event.button == 0)
            {
                OnClick?.Invoke(this);
            }
            else if (@event.button == 1)
            {
                _menu.Open();
            }
        }

        dragManager.OnGUI(rect, this);

        rect.ButtonGhostly();
    }

    public abstract void Draw(Rect rect);

    public abstract void Focus();

    public abstract void Unfocus();

    public abstract void Dispose();
}
