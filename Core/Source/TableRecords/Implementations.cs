using System;
using RimWorld;
using Verse;

namespace Stats.TableRecords;

public readonly record struct ThingDefTableRecord :
    IThingDefTableRecord
{
    public Def Def => ThingDef;
    public BuildableDef BuildableDef => ThingDef;
    public ThingDef ThingDef { get; }
    public StatRequest StatRequest { get; }

    public ThingDefTableRecord(ThingDef thingDef, ThingDef? stuffDef = null)
    {
        ThingDef = thingDef;
        StatRequest = StatRequest.For(thingDef, stuffDef);
    }
}

public readonly record struct PlantDefTableRecord :
    IPlantDefTableRecord
{
    public Def Def => ThingDef;
    public BuildableDef BuildableDef => ThingDef;
    public ThingDef ThingDef { get; }
    public StatRequest StatRequest { get; }
    public PlantProperties PlantProperties { get; }

    public PlantDefTableRecord(ThingDef thingDef, PlantProperties plantProperties)
    {
        ThingDef = thingDef;
        StatRequest = StatRequest.For(thingDef, null);
        PlantProperties = plantProperties;
    }
}

public readonly record struct PawnDefTableRecord :
    IPawnDefTableRecord,
    IMilkableDefTableRecord,
    IEggLayerDefTableRecord,
    IShearableDefTableRecord
{
    public Def Def => ThingDef;
    public BuildableDef BuildableDef => ThingDef;
    public ThingDef ThingDef { get; }
    public StatRequest StatRequest { get; }
    public RaceProperties RaceProperties { get; }
    private readonly Lazy<CompProperties_Milkable?> _milkableCompProperties;
    public CompProperties_Milkable? MilkableCompProperties => _milkableCompProperties.Value;
    private readonly Lazy<CompProperties_EggLayer?> _eggLayerCompProperties;
    public CompProperties_EggLayer? EggLayerCompProperties => _eggLayerCompProperties.Value;
    private readonly Lazy<CompProperties_Shearable?> _shearableCompProperties;
    public CompProperties_Shearable? ShearableCompProperties => _shearableCompProperties.Value;

    public PawnDefTableRecord(ThingDef thingDef, RaceProperties raceProperties)
    {
        ThingDef = thingDef;
        RaceProperties = raceProperties;
        StatRequest = StatRequest.For(thingDef, null);
        _milkableCompProperties = new Lazy<CompProperties_Milkable?>(thingDef.GetCompProperties<CompProperties_Milkable>);
        _eggLayerCompProperties = new Lazy<CompProperties_EggLayer?>(thingDef.GetCompProperties<CompProperties_EggLayer>);
        _shearableCompProperties = new Lazy<CompProperties_Shearable?>(thingDef.GetCompProperties<CompProperties_Shearable>);
    }
}

public readonly record struct ApparelDefTableRecord :
    IApparelDefTableRecord
{
    public Def Def => ThingDef;
    public BuildableDef BuildableDef => ThingDef;
    public ThingDef ThingDef { get; }
    public StatRequest StatRequest { get; }
    public ApparelProperties ApparelProperties { get; }

    public ApparelDefTableRecord(ThingDef thingDef, ApparelProperties apparelProperties, ThingDef? stuffDef = null)
    {
        ThingDef = thingDef;
        StatRequest = StatRequest.For(thingDef, stuffDef);
        ApparelProperties = apparelProperties;
    }
}

public readonly record struct BuildingDefTableRecord :
    IBuildingDefTableRecord,
    IPowerTraderDefTableRecord,
    IRefuelableDefTableRecord
{
    public Def Def => ThingDef;
    public BuildableDef BuildableDef => ThingDef;
    public ThingDef ThingDef { get; }
    public StatRequest StatRequest { get; }
    public BuildingProperties BuildingProperties { get; }
    private readonly Lazy<CompProperties_Power?> _powerCompProperties;
    public CompProperties_Power? PowerCompProperties => _powerCompProperties.Value;
    private readonly Lazy<CompProperties_Refuelable?> _refuelableCompProperties;
    public CompProperties_Refuelable? RefuelableCompProperties => _refuelableCompProperties.Value;


    public BuildingDefTableRecord(ThingDef thingDef, BuildingProperties buildingProperties, ThingDef? stuffDef = null)
    {
        ThingDef = thingDef;
        StatRequest = StatRequest.For(thingDef, stuffDef);
        BuildingProperties = buildingProperties;
        _powerCompProperties = new Lazy<CompProperties_Power?>(thingDef.GetCompProperties<CompProperties_Power>);
        _refuelableCompProperties = new Lazy<CompProperties_Refuelable?>(thingDef.GetCompProperties<CompProperties_Refuelable>);
    }
}

public readonly record struct RangedWeaponDefTableRecord :
    IRangedWeaponDefTableRecord
{
    public Def Def => ThingDef;
    public BuildableDef BuildableDef => ThingDef;
    public ThingDef ThingDef { get; }
    public StatRequest StatRequest { get; }
    public StatRequest RangedWeaponStatRequest => StatRequest;
    public VerbProperties PrimaryVerbProperties { get; }

    public RangedWeaponDefTableRecord(ThingDef thingDef, VerbProperties primaryVerbProperties, ThingDef? stuffDef = null)
    {
        ThingDef = thingDef;
        StatRequest = StatRequest.For(thingDef, stuffDef);
        PrimaryVerbProperties = primaryVerbProperties;
    }
}

public readonly record struct TurretDefTableRecord :
    ITurretDefTableRecord
{
    public Def Def => ThingDef;
    public BuildableDef BuildableDef => ThingDef;
    public ThingDef ThingDef { get; }
    public StatRequest StatRequest { get; }
    public StatRequest RangedWeaponStatRequest { get; }
    public BuildingProperties BuildingProperties { get; }
    public VerbProperties PrimaryVerbProperties { get; }

    public TurretDefTableRecord(
        ThingDef thingDef,
        BuildingProperties buildingProperties,
        ThingDef gunDef,
        VerbProperties verbProperties,
        ThingDef? stuffDef = null)
    {
        ThingDef = thingDef;
        StatRequest = StatRequest.For(thingDef, stuffDef);
        RangedWeaponStatRequest = StatRequest.For(gunDef, null);
        BuildingProperties = buildingProperties;
        PrimaryVerbProperties = verbProperties;
    }
}

public readonly record struct ThingTableRecord :
    IThingTableRecord
{
    public Def Def => Thing.def;
    public BuildableDef BuildableDef => Thing.def;
    public ThingDef ThingDef => Thing.def;
    public StatRequest StatRequest { get; }
    public Thing Thing { get; }

    public ThingTableRecord(Thing thing)
    {
        Thing = thing;
        StatRequest = StatRequest.For(Thing);
    }
}
