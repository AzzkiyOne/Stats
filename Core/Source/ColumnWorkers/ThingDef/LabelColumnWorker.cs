using System.Collections.Generic;
using Stats.ColumnWorkers.Cells;
using Stats.Filters;
using Stats.TableRecords;
using Stats.TableWorkers;
using Stats.Utils;
using Stats.Utils.Extensions;
using Stats.Utils.Widgets;
using UnityEngine;
using Verse;
using static Stats.GUIStyles.TableCell;

namespace Stats.ColumnWorkers.ThingDef;

// Note to myself: Don't remove stuff label. It's important because
// modded stuffs may have the same color as vanilla ones or other modded stuffs.
// Replacing label with icon won't do, because ex. all of the leathers have the same
// icon but of different color.
public sealed class LabelColumnWorker<TRecord>(ColumnDef columnDef) :
    ColumnWorker<TRecord, LabelColumnWorker<TRecord>.LabelCell>(columnDef, ColumnType.String)
        where TRecord :
            IThingDefTableRecord
{
    public override bool ShouldDrawCellsNow => Event.current is { type: EventType.Repaint or EventType.MouseDown or EventType.MouseUp };

    protected override LabelCell MakeCell(TRecord record)
    {
        return new LabelCell(record.ThingDef, record.StatRequest.StuffDef);
    }

    public override ICollection<CellField> GetCellFields(TableWorker tableWorker)
    {
        Filter textFieldFilter = new StringFilter((int row) => this[row].Text ?? "");
        int Compare(int row1, int row2) => Comparer<string?>.Default.Compare(this[row1].Text, this[row2].Text);
        CellField textField = new(null, textFieldFilter, Compare);

        return [textField];
    }

    //IEnumerable<ObjectTableWidget.ColumnPart> IColumnWorker<VirtualThing>.GetObjectProps()
    //{
    //    yield return new(new Label("Label"), new StringFilter(cell => ((Cell)cell).Text));

    //    var typeFilterOptions = contextObjects
    //        .Select(@object => @object.Def)
    //        .Distinct()
    //        .OrderBy(thingDef => thingDef.label)
    //        .Select<ThingDef, NTMFilterOption<ThingDef>>(
    //            thingDef => new(thingDef, thingDef.LabelCap, new ThingDefIcon(thingDef))
    //        );
    //    yield return new(new Label("Type"), new OTMFilter<ThingDef>(cell => ((Cell)cell).Def, typeFilterOptions));

    //    var filterWidget_Researched = new BooleanFilter(
    //        cell => ((Cell)cell).Def.GetResearchProjectDefs()?.All(researchProjectDef => researchProjectDef.IsFinished) is true or null
    //    );
    //    Globals.Events.OnResearchCompleted += () =>
    //    {
    //        if (filterWidget_Researched.IsActive)
    //        {
    //            filterWidget_Researched.NotifyChanged();
    //        }
    //    };
    //    yield return new(new Label("Researched"), filterWidget_Researched);

    //    if (contextObjects.Any(@object => @object.StuffDef != null))
    //    {
    //        yield return new(new Label("Distinct"), new StuffedVariantsDisplayModeToggleButton());

    //        var stuffFilterOptions = contextObjects
    //            .Select(@object => @object.StuffDef)
    //            .Distinct()
    //            .OrderBy(thingDef => thingDef?.label)
    //            .Select<ThingDef?, NTMFilterOption<ThingDef?>>(
    //                thingDef => thingDef == null
    //                    ? new()
    //                    : new(thingDef, thingDef.LabelCap, new ThingDefIcon(thingDef))
    //            );
    //        var stuffFilter = new OTMFilter<ThingDef?>(cell => ((Cell)cell).StuffDef, stuffFilterOptions);
    //        yield return new(new Label("Material"), stuffFilter);
    //    }
    //}

    public readonly struct LabelCell : ICell
    {
        public float Width { get; }
        public bool IsRefreshable => false;
        public readonly string? Text;

        private readonly Verse.ThingDef? _thingDef;
        private readonly Verse.ThingDef? _stuffDef;
        private readonly ThingDefIcon? _icon;
        private readonly float _iconWidth;

        public LabelCell(Verse.ThingDef thingDef, Verse.ThingDef? stuffDef = null)
        {
            _thingDef = thingDef;
            _stuffDef = stuffDef;
            Text = stuffDef == null
                ? thingDef.LabelCap.RawText
                : $"{stuffDef.LabelAsStuff.CapitalizeFirst()} {thingDef.label}";
            float textWidth = Text.CalcSize(StringNoPad).x;
            _icon = new ThingDefIcon(thingDef, stuffDef);
            _iconWidth = _icon.Size.x;
            Width = _iconWidth + ContentSpacing + textWidth;
        }

        public void Draw(Rect rect)
        {
            if (_thingDef != null)
            {
                rect
                    .ContractedByObjectTableCellPadding()
                    .CutLeft(out Rect iconRect, _iconWidth)
                    .CutLeft(ContentSpacing)
                    .TakeRest(out Rect labelRect);

                if (Event.current.type == EventType.Repaint)
                {
                    _icon!.Draw(iconRect);
                    Text!.Draw(labelRect, StringNoPad);
                }

                bool iconWasClicked = iconRect.ButtonGhostly();
                if (iconWasClicked)
                {
                    _thingDef.OpenInfoDialog(_stuffDef);
                }
            }
        }

        //public Cell(VirtualThing thing)
        //{
        //    Def = thing.Def;
        //    StuffDef = thing.StuffDef;
        //    IsMadeFromDefaultStuff = thing.StuffDef == thing.Def.GetDefaultStuff();

        //    var text = thing.StuffDef == null
        //        ? thing.Def.LabelCap.RawText
        //        : $"{thing.StuffDef.LabelAsStuff.CapitalizeFirst()} {thing.Def.label}";
        //    var widget = new HorizontalContainer([
        //        new ThingDefIcon(thing.Def, thing.StuffDef)
        //        .PaddingAbs(2f)
        //        .SizeAbs(Verse.Text.LineHeight + ObjectTableWidget.CellPadVer * 2f)
        //        .ToButtonGhostly(() => Draw.DefInfoDialog(thing.Def, thing.StuffDef)),
        //        new Label(text).PaddingAbs(0f, ObjectTableWidget.CellPadVer),
        //    ], Globals.GUI.Pad)
        //    .PaddingAbs(ObjectTableWidget.CellPadHor, 0f)
        //    .Tooltip(thing.Def.description);

        //    Widget = widget;
        //    Text = text;
        //}
    }

    /*
    
    TODO: 
    
    Instead of having this "filter", have two sets of tables for thing defs that can be made from stuff:
    - A table that lists every def + stuff variant. Default columns are ones whose values depend on stuff.
    - A table that lists bases (without default stuff). Default columns are ones whose values do not depend on stuff.

    Note:

    This being a filter is a hack. It doesn't work in "OR" mode and is semantically incorrect.
    
    There are 2 ways of fixing this issue.

    The easy way is to introduce some special/pre filters that would be displayed in window's toolbar 
    and be applied separatedly.

    The hard way is to allow for grouping table's rows by a column.
    The issue that is being solved by this filter is "show me only values that do not depend on stuff".
    Grouping can be implemented by implementing "Equals" on a cell.
    
    */
    //private sealed class StuffedVariantsDisplayModeToggleButton : FilterWidget
    //{
    //    private bool _IsActive = false;
    //    public override bool IsActive => _IsActive;
    //    public override event Action? OnChange;
    //    private Texture2D Texture => _IsActive ? Verse.Widgets.CheckboxOnTex : Verse.Widgets.CheckboxOffTex;
    //    private static readonly TipSignal Manual =
    //        "Click to show only distinct item material variants.\n\n" +
    //        "Material for each distinct variant is chosen based on item's type definition.";
    //    public StuffedVariantsDisplayModeToggleButton()
    //    {
    //    }
    //    protected override Vector2 GetSize()
    //    {
    //        return new Vector2(Text.LineHeight, Text.LineHeight);
    //    }
    //    public override void Draw(Rect rect, Vector2 _)
    //    {
    //        var origTextAnchor = Text.Anchor;
    //        Text.Anchor = TextAnchor.LowerLeft;

    //        var origGUIColor = GUI.color;
    //        if (_IsActive == false)
    //        {
    //            GUI.color = Globals.GUI.TextColorSecondary;
    //        }

    //        if (Widgets.Draw.ButtonImageSubtle(rect, Texture))
    //        {
    //            _IsActive = !_IsActive;

    //            OnChange?.Invoke();
    //        }

    //        Text.Anchor = origTextAnchor;
    //        GUI.color = origGUIColor;

    //        TooltipHandler.TipRegion(rect, Manual);
    //    }
    //    public override bool Eval(ObjectTableWidget.Cell cell)
    //    {
    //        if (_IsActive)
    //        {
    //            // Do not filter out stuffless things.
    //            // - You can filter them out with stuff filter.
    //            // - There are cases where one may want to compare
    //            //   things by stats unrelated to stuff. Ex. equipped
    //            //   stat offsets.
    //            return ((Cell)cell).IsMadeFromDefaultStuff;
    //        }

    //        return true;
    //    }
    //    public override void Reset()
    //    {
    //    }
    //    public override void NotifyChanged()
    //    {
    //        OnChange?.Invoke();
    //    }
    //}
}
