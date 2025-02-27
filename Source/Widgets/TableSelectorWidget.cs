using System;
using System.Linq;
using UnityEngine;
using Verse;

namespace Stats;

internal class TableSelectorWidget
{
    public TableDef CurTableDef { get; private set; }
    private string _curLabel;
    private string CurLabel
    {
        get => _curLabel;
        set
        {
            _curLabel = value;
            CurLabelWidth = Text.CalcSize(value).x;
        }
    }
    private float CurLabelWidth;
    private FloatMenu Menu { get; }
    public TableSelectorWidget()
    {
        var menuOptions = DefDatabase<TableDef>
            .AllDefs
            .Where(tableDef => tableDef.columns.Count > 0)
            .Select(tableDef =>
            {
                var label = tableDef.LabelCap;
                var curParent = tableDef.parent;

                while (curParent != null)
                {
                    // Maybe better use StringBuilder.
                    label = $"{curParent.LabelCap} / " + label;
                    curParent = curParent.parent;
                }

                return new FloatMenuOption(
                    label,
                    () =>
                    {
                        CurTableDef = tableDef;
                        CurLabel = label;
                    },
                    tableDef.Icon,
                    tableDef.IconColor
                );
            })
            .OrderBy(opt => opt.Label)
            .ToList();

        Menu = new(menuOptions);
        menuOptions.First().action();
    }
    public void Draw(Rect targetRect)
    {
        var targetRectWidth = targetRect.height
                              + GenUI.Pad * 3
                              + CurLabelWidth;
        var labelDoesntFit = targetRect.width < targetRectWidth;

        targetRect = targetRect.CutByX(Math.Min(targetRectWidth, targetRect.width));

        if (labelDoesntFit)
        {
            TooltipHandler.TipRegion(targetRect, CurLabel);
        }

        Widgets.DrawHighlightIfMouseover(targetRect);

        if (Widgets.ButtonInvisible(targetRect))
        {
            Find.WindowStack.Add(Menu);
        }

        targetRect.PadLeft(GenUI.Pad);

        GUI.color = CurTableDef.IconColor;
        Widgets.DrawTextureFitted(
            targetRect.CutByX(targetRect.height),
            CurTableDef.Icon,
            1f
        );
        GUI.color = Color.white;

        Widgets.Label(
            targetRect.ContractedBy(GenUI.Pad, 0f),
            CurLabel
        );
    }
}
