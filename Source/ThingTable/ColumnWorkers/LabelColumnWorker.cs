using System;
using System.Collections.Generic;
using System.Linq;
using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats.ThingTable;

public sealed class LabelColumnWorker : ColumnWorker<ThingAlike>
{
    private static readonly Func<ThingAlike, string> GetThingLabel =
        FunctionExtensions.Memoized((ThingAlike thing) =>
        {
            // Note to myself: Don't remove stuff label. It's important because
            // modded stuffs may have the same color as vanilla ones or other modded stuffs.
            // Replacing label with icon won't do, because ex. all of the leathers have the same
            // icon but of different color.
            return thing.StuffDef == null
                ? thing.Def.LabelCap.RawText
                : $"{thing.StuffDef.LabelCap} {thing.Def.label}";
        });
    public LabelColumnWorker(ColumnDef columnDef) : base(columnDef, ColumnCellStyle.String)
    {
    }
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        void openDefInfoDialog()
        {
            Draw.DefInfoDialog(thing.Def, thing.StuffDef);
        }

        return new HorizontalContainer(
            [
                new ThingIcon(thing.Def, thing.StuffDef).ToButtonGhostly(openDefInfoDialog),
                new Label(GetThingLabel(thing)),
            ],
            Globals.GUI.Pad
        )
        .Tooltip(thing.Def.description);
    }
    public override FilterWidget<ThingAlike> GetFilterWidget(IEnumerable<ThingAlike> tableRecords)
    {
        var labelFilter = new StringFilter<ThingAlike>(GetThingLabel);

        if (tableRecords.Any(record => record.StuffDef != null))
        {
            var stuffDefs = tableRecords
                .Select(thing => thing.StuffDef)
                .Distinct()
                .OrderBy(stuffDef => stuffDef?.label);
            var stuffFilter = new OneToManyFilter<ThingAlike, ThingDef?>(
                thing => thing.StuffDef,
                stuffDefs,
                stuffDef => stuffDef == null
                    ? new Label("")
                    : new HorizontalContainer(
                        [
                            new ThingIcon(stuffDef),
                            new Label(stuffDef.LabelCap).WidthRel(1f)
                        ],
                        Globals.GUI.PadSm,
                        true
                    )
            );

            return new CompositeFilter<ThingAlike>([
                new StuffedVariantsDisplayModeToggleButton(),
                stuffFilter.Tooltip("Filter by material."),
                labelFilter.WidthRel(1f).Tooltip("Filter by label.")
            ]);
        }

        return labelFilter;
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return GetThingLabel(thing1).CompareTo(GetThingLabel(thing2));
    }

    private sealed class StuffedVariantsDisplayModeToggleButton : FilterWidget<ThingAlike>
    {
        private bool _IsActive = false;
        public override bool IsActive => _IsActive;
        public override event Action<FilterWidget<ThingAlike>>? OnChange;
        private const string ButtonTextActive = "D";
        private const string ButtonTextDisabled = "A";
        private static readonly TipSignal Manual =
            "Click to switch material variants display mode:\n" +
            "A - \"All\"'. Show every variant.\n" +
            "D - \"Distinct\". Show only distinct variants. Material for each distinct variant is chosen based on item's type definition.";
        protected override Vector2 CalcSize()
        {
            return new Vector2(Text.LineHeight, Text.LineHeight);
        }
        public override void Draw(Rect rect, Vector2 _)
        {
            var origTextAnchor = Text.Anchor;
            Text.Anchor = TextAnchor.LowerCenter;

            if (Widgets.Draw.ButtonTextSubtle(rect, _IsActive ? ButtonTextActive : ButtonTextDisabled))
            {
                _IsActive = !_IsActive;

                OnChange?.Invoke(this);
            }

            Text.Anchor = origTextAnchor;

            TooltipHandler.TipRegion(rect, Manual);
        }
        public override bool Eval(ThingAlike thing)
        {
            if (_IsActive)
            {
                // Do not filter out stuffless things.
                // - You can filter them out with stuff filter.
                // - There are cases where one may want to compare
                //   things by stats unrelated to stuff. Ex. equipped
                //   stat offsets.
                return thing.StuffDef == thing.Def.GetDefaultStuff();
            }

            return true;
        }
        public override void Reset()
        {
            _IsActive = false;

            OnChange?.Invoke(this);
        }
    }
}
