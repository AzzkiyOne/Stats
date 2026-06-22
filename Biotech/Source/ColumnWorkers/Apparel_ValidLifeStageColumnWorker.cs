using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stats.TableRecords.ThingDef;
using Stats.Widgets_Legacy;
using Verse;

namespace Stats.Compat.Biotech;

public sealed class Apparel_ValidLifeStageColumnWorker : ColumnWorker<VirtualThing>
{
    public Apparel_ValidLifeStageColumnWorker(ColumnDef columnDef) : base(columnDef, CellStyleType.String, TODO)
    {
    }
    private static readonly Func<ThingDef, HashSet<DevelopmentalStage>> GetValidLifeStages =
    FunctionExtensions.Memoized((ThingDef thingDef) =>
    {
        var lifeStages = new HashSet<DevelopmentalStage>();

        if (thingDef.apparel == null)
        {
            return lifeStages;
        }

        var enumerator = thingDef.apparel.developmentalStageFilter.Enumerate().GetEnumerator();

        while (enumerator.MoveNext())
        {
            lifeStages.Add(enumerator.Current);
        }

        return lifeStages;
    });
    private static readonly Func<DevelopmentalStage, string> GetLifeStageString =
    FunctionExtensions.Memoized((DevelopmentalStage lifeStage) =>
    {
        return lifeStage.ToString().Translate().CapitalizeFirst().RawText;
    });
    public override ObjectTable.Cell GetCell(VirtualThing thing)
    {
        var lifeStages = GetValidLifeStages(thing.Def);

        return new Cell(lifeStages);
    }
    public override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<VirtualThing> contextObjects)
    {
        var filterOptions = contextObjects
            .SelectMany(thing => GetValidLifeStages(thing.Def))
            .Distinct()
            .OrderBy(lifeStage => lifeStage)
            .Select(lifeStage => new NTMFilterOption<DevelopmentalStage>(lifeStage, GetLifeStageString(lifeStage)));

        yield return new(Def.Title, new MTMFilter<Cell, DevelopmentalStage>(cell => cell.Value, filterOptions, this));
    }

    private sealed class Cell : ObjectTable.WidgetCell
    {
        protected override Widget? Widget { get; set; }
        public override event Action? OnChange;
        public HashSet<DevelopmentalStage> Value { get; }
        public string? Text { get; }
        public Cell(HashSet<DevelopmentalStage> value)
        {
            Value = value;

            if (value.Count > 0)
            {
                var stringBuilder = new StringBuilder();

                foreach (var lifeStage in value.OrderBy(lifeStage => lifeStage))
                {
                    stringBuilder.AppendInNewLine(GetLifeStageString(lifeStage));
                }

                var text = stringBuilder.ToString();

                Widget = new Label(text);
                Text = text;
            }
        }
        public override int CompareTo(ObjectTable.Cell cell)
        {
            return Comparer<string?>.Default.Compare(Text, ((Cell)cell).Text);
        }
    }
}
