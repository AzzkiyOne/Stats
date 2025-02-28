using System;
using System.Linq;
using UnityEngine;
using Verse;

namespace Stats;

internal class TableSelectorWidget
{
    private TableDef _CurTableDef;
    public TableDef CurTableDef
    {
        get => _CurTableDef;
        private set
        {
            _CurTableDef = value;
            CurLabelWidth = Text.CalcSize(value.LabelCap).x;
        }
    }
    private float CurLabelWidth;
    private FloatMenu Menu { get; }
    public TableSelectorWidget()
    {
        var menuOptions = DefDatabase<TableDef>
            .AllDefs
            .Where(tableDef => tableDef.columns.Count > 0)
            .Select(tableDef => new FloatMenuOption(
                tableDef.LabelCap,
                () => CurTableDef = tableDef,
                tableDef.Icon,
                tableDef.IconColor
            ))
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
            TooltipHandler.TipRegion(targetRect, CurTableDef.LabelCap);
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
            CurTableDef.LabelCap
        );
    }
}
