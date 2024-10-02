using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Stats;

internal class TablesBrowserWidget
{
    private const float RowHeight = 22f;
    private const float LabelPadding = 5f;
    private const float IndentSize = 20f;
    private Vector2 ScrollPosition;
    public TableDef CurTable { get; private set; }
    private int debug_rowsDrawn = 0;
    private int TotalRowsDisplayed = 0;
    private List<TableDef> RootTableDefs;
    public TablesBrowserWidget()
    {
        CurTable = TableDefOf.All;
        RootTableDefs = DefDatabase<TableDef>.AllDefs.Where(tableDef => tableDef.parent == null).ToList();
    }
    public void Draw(Rect targetRect)
    {
        using (new TextAnchorCtx(TextAnchor.MiddleLeft))
        {
            // Scroll area size correction only works because of how rows are "culled" at the top.
            // We don't render them, but still counting.
            var displayedRowsHeight = RowHeight * TotalRowsDisplayed;
            var contentRect = new Rect(
                0f,
                0f,
                targetRect.width,
                displayedRowsHeight >= targetRect.height
                    ? displayedRowsHeight + targetRect.height
                    : displayedRowsHeight
            );
            var currY = 0f;

            Widgets.BeginScrollView(targetRect, ref ScrollPosition, contentRect);

            debug_rowsDrawn = 0;
            TotalRowsDisplayed = 0;

            foreach (var tableDef in RootTableDefs)
            {
                DrawRows(targetRect, ref currY, tableDef);
            }

            Widgets.EndScrollView();

            Debug.TryDrawUIDebugInfo(targetRect, debug_rowsDrawn + "");
        }
    }
    private void DrawRows(
        Rect parentRect,
        ref float currY,
        TableDef tableDef,
        float treeLevel = 0
    )
    {
        TotalRowsDisplayed++;

        var indentAmount = treeLevel * IndentSize;
        var rowRect = new Rect(
            indentAmount,
            currY,
            parentRect.width - indentAmount,
            RowHeight
        );

        // Culling
        if (rowRect.y - ScrollPosition.y > parentRect.height)
        {
            return;
        }

        if (rowRect.yMax - ScrollPosition.y > 0)
        {
            var iconRect = rowRect.LeftPartPixels(RowHeight);
            var labelRect = rowRect
                .RightPartPixels(rowRect.width - iconRect.width)
                .ContractedBy(LabelPadding, 0);
            string labelText = Debug.InDebugMode ? tableDef.defName : tableDef.LabelCap;

            Widgets.DrawTextureFitted(iconRect, tableDef.Icon, 0.9f);
            Widgets.Label(labelRect, labelText);

            Widgets.DrawHighlightIfMouseover(rowRect);

            if (Widgets.ButtonInvisible(rowRect))
            {
                CurTable = tableDef;
            }

            if (CurTable == tableDef)
            {
                Widgets.DrawHighlight(rowRect);
            }

            debug_rowsDrawn++;
        }

        currY += RowHeight;

        foreach (var childTableDef in tableDef.Children)
        {
            DrawRows(parentRect, ref currY, childTableDef, treeLevel + 1);
        }
    }
}
