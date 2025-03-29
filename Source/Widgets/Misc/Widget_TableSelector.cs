using System;
using System.Linq;
using UnityEngine;
using Verse;

namespace Stats;

internal sealed class Widget_TableSelector
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
    public Widget_TableSelector()
    {
        var menuOptions =
            DefDatabase<TableDef>
            .AllDefs
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

        Widgets.DrawLightHighlight(targetRect);

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

        Text.Anchor = TextAnchor.MiddleLeft;
        Widgets.Label(
            targetRect.ContractedBy(GenUI.Pad, 0f),
            CurTableDef.LabelCap
        );
        Text.Anchor = Constants.DefaultTextAnchor;
    }
}
