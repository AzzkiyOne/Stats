using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stats.ThingDefTable;
using UnityEngine;
using Verse;

namespace Stats;

internal class Filters<T>
{
    private List<IFilter<T>> CurFilters { get; } = [];
    private FloatMenu Menu { get; }
    private Action<List<IFilter<T>>> OnUpdate { get; }
    public Filters(Action<List<IFilter<T>>> onUpdate)
    {
        OnUpdate = onUpdate;

        var menuOptions = new List<FloatMenuOption>();

        foreach (var column in DefDatabase<GenTable.ColumnDef>.AllDefs)
        {
            if (
                column is GenTable.IColumn<T>
                && column is IFilterProvider<T> _column
            )
            {
                menuOptions.Add(new(
                    column.Label,
                    () => { CurFilters.Add(_column.GetFilter()); }
                ));
            }
        }

        Menu = new(menuOptions);
    }
    public void Draw(Rect targetRect)
    {
        var curY = targetRect.y;

        foreach (var filter in CurFilters)
        {
            filter.Draw(targetRect.CutFromY(ref curY, 25f));
            curY += 5f;
        }

        if (Widgets.ButtonText(targetRect.CutFromY(ref curY, 30f), "Add"))
        {
            Find.WindowStack.Add(Menu);
        }

        if (Widgets.ButtonText(targetRect.CutFromY(ref curY, 30f), "Update"))
        {
            OnUpdate(CurFilters);
        }
    }
}
