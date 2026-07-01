namespace Stats.Columns.AnimalDef;

//public sealed class Animal_BiomesColumnWorker : StaticColumnWorker<DefBasedObject,>
//{
//    public override ColumnDef Def => columnDef;

//    public Animal_BiomesColumnWorker(ColumnDef columnDef) : base(columnDef, CellStyleType.String, TODO)
//    {
//    }
//    // This exists mainly for consistency, for when this column worker is used by several tables.
//    private static readonly Func<ThingDef, List<BiomeRecord>> GetBiomeRecords =
//    FunctionExtensions.Memoized((ThingDef thingDef) =>
//    {
//        var biomeRecords = new List<BiomeRecord>();
//        var raceProps = thingDef.race;

//        if (raceProps?.Animal == true)
//        {
//            foreach (var biomeDef in DefDatabase<BiomeDef>.AllDefsListForReading)
//            {
//                var animalCommonality = biomeDef.CommonalityOfAnimal(raceProps.AnyPawnKind);

//                if (animalCommonality > 0f)
//                {
//                    var biomeRecord = new BiomeRecord(biomeDef, animalCommonality);

//                    biomeRecords.Add(biomeRecord);
//                }
//            }
//        }

//        biomeRecords.SortByDescending(biomeRecord => biomeRecord.Commonality);

//        return biomeRecords;
//    });
//    public override ObjectTableWidget.Cell GetCell(VirtualThing thing)
//    {
//        var biomeRecords = GetBiomeRecords(thing.Def);

//        return new Cell(biomeRecords);
//    }
//    public override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<VirtualThing> contextObjects)
//    {
//        var options = contextObjects
//            .SelectMany(thing => GetBiomeRecords(thing.Def).Select(biomeRecord => biomeRecord.BiomeDef).ToHashSet())
//            .Distinct()
//            .OrderBy(def => def.label)
//            .Select<BiomeDef, NTMFilterOption<BiomeDef>>(
//                def => def == null ? new() : new(def, def.LabelCap)
//            );

//        yield return new(Def.Title, new MTMFilter<Cell, BiomeDef>(cell => cell.Biomes, options, this));
//    }

//    private sealed class Cell : ObjectTableWidget.WidgetCell
//    {
//        protected override Widget? Widget { get; set; }
//        public override event Action? OnChange;
//        public HashSet<BiomeDef> Biomes { get; }
//        public float AverageCommonality { get; }
//        public Cell(List<BiomeRecord> biomeRecords)
//        {
//            if (biomeRecords.Count > 0)
//            {
//                var text = biomeRecords[0].ToString();
//                string? tooltip = null;

//                if (biomeRecords.Count > 1)
//                {
//                    text += $" ({biomeRecords.Count})".Colorize(Globals.GUI.TextColorSecondary);

//                    var stringBuilder = new StringBuilder();

//                    foreach (var biomeRecord in biomeRecords)
//                    {
//                        stringBuilder.AppendLine(biomeRecord.ToString());
//                    }

//                    tooltip = stringBuilder.ToString();
//                }

//                var animalCommonalitySum = 0f;

//                foreach (var biomeRecord in biomeRecords)
//                {
//                    animalCommonalitySum += biomeRecord.Commonality;
//                }

//                var averageCommonality = animalCommonalitySum / biomeRecords.Count;

//                var biomeDefs = biomeRecords.Select(biomeRecord => biomeRecord.BiomeDef).ToHashSet();

//                Widget widget = new Label(text).PaddingAbs(ObjectTableWidget.CellPadHor, ObjectTableWidget.CellPadVer);

//                if (tooltip != null)
//                {
//                    widget = widget.Tooltip(tooltip);
//                }

//                Widget = widget;
//                Biomes = biomeDefs;
//            }
//            else
//            {
//                Biomes = [];
//            }
//        }
//        public override int CompareTo(ObjectTableWidget.Cell cell)
//        {
//            return AverageCommonality.CompareTo(((Cell)cell).AverageCommonality);
//        }
//    }

//    private readonly record struct BiomeRecord(BiomeDef BiomeDef, float Commonality)
//    {
//        public override string ToString()
//        {
//            return $"{BiomeDef.LabelCap}: {Commonality.ToString("0.###").Colorize(Globals.GUI.TextColorHighlight)}";
//        }
//    }
//}
