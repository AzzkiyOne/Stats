using System;
using System.Collections.Generic;
using Stats.Table;
using Stats.Table.Columns;
using UnityEngine;
using Verse;

namespace Stats;

internal class FiltersEditor<T>
{
    private List<IFilter<T>> CurFilters { get; } = [];
    private FloatMenu Menu { get; }
    private Action<List<IFilter<T>>> OnUpdate { get; }
    public FiltersEditor(Action<List<IFilter<T>>> onUpdate)
    {
        OnUpdate = onUpdate;

        var menuOptions = new List<FloatMenuOption>();

        foreach (var column in DefDatabase<Column>.AllDefs)
        {
            if (column is IFilterProvider<T> _column)
            {
                menuOptions.Add(new(
                    column.Label,
                    () =>
                    {
                        CurFilters.Add(_column.GetFilter());
                        OnUpdate(CurFilters);
                    }
                ));
            }
        }

        Menu = new(menuOptions);
    }
    public void Draw(Rect targetRect)
    {
        var curY = targetRect.y;

        if (Widgets.ButtonText(targetRect.CutFromY(ref curY, 30f), "Add"))
        {
            Find.WindowStack.Add(Menu);
        }

        foreach (var filter in CurFilters)
        {
            var labelRect = targetRect.CutFromY(ref curY, 30);
            var curX = labelRect.x;

            using (new TextAnchorCtx(TextAnchor.LowerLeft))
            {
                Widgets.Label(labelRect.CutFromX(ref curX, labelRect.width - labelRect.height).ContractedBy(5f, 0f), filter.Column.Label);
            }

            if (Widgets.ButtonImageFitted(labelRect.CutFromX(ref curX), Widgets.CheckboxOffTex))
            {
                CurFilters.Remove(filter);
                OnUpdate(CurFilters);

                return;
            }

            if (filter.Draw(targetRect.CutFromY(ref curY, 30f)))
            {
                OnUpdate(CurFilters);
            }
        }
    }
}
