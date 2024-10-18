using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Stats;

internal class TableSelectorWidget
{
    public TableDef CurTableDef { get; private set; }
    private FloatMenu Menu { get; }
    public TableSelectorWidget()
    {
        var tables = DefDatabase<TableDef>
            .AllDefs
            .Where(tableDef => tableDef.columns.Count > 0)
            .OrderBy(tableDef => tableDef.Path);
        List<FloatMenuOption> menuOptions = tables
            .Select(tableDef =>
                new FloatMenuOption(tableDef.Path, () => CurTableDef = tableDef)
            )
            .ToList();

        CurTableDef = tables.First();
        Menu = new(menuOptions);
    }
    public void Draw(Rect targetRect)
    {
        Widgets.DrawHighlightIfMouseover(targetRect);

        if (Widgets.ButtonInvisible(targetRect))
        {
            Find.WindowStack.Add(Menu);
        }

        targetRect.PadLeft(GenUI.Pad);

        Widgets.DrawTextureFitted(
            targetRect.CutByX(targetRect.height),
            CurTableDef.Icon,
            0.9f
        );
        Widgets.Label(
            targetRect.ContractedBy(GenUI.Pad, 0f),
            CurTableDef.Path
        );
    }
}
