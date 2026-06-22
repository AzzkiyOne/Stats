using RimWorld;
using Verse;

namespace Stats.TableRecords;

public interface IDefTableRecord
{
    Def Def { get; }
}

public interface IBuildableDefTableRecord :
    IDefTableRecord
{
    BuildableDef BuildableDef { get; }
    StatRequest StatRequest { get; }
}

public interface IThingDefTableRecord :
    IBuildableDefTableRecord
{
    ThingDef ThingDef { get; }
}

public interface IPlantDefTableRecord :
    IThingDefTableRecord
{
    PlantProperties PlantProperties { get; }
}

public interface IPawnDefTableRecord :
    IThingDefTableRecord
{
    RaceProperties RaceProperties { get; }
}

public interface IApparelDefTableRecord :
    IThingDefTableRecord
{
    ApparelProperties ApparelProperties { get; }
}

public interface IBuildingDefTableRecord :
    IThingDefTableRecord
{
    BuildingProperties BuildingProperties { get; }
}

public interface IRangedWeaponDefTableRecord :
    IThingDefTableRecord
{
    VerbProperties PrimaryVerbProperties { get; }
    StatRequest RangedWeaponStatRequest { get; }
}

public interface ITurretDefTableRecord :
    IBuildingDefTableRecord,
    IRangedWeaponDefTableRecord
{
}

public interface IEggLayerDefTableRecord :
    IThingDefTableRecord
{
    CompProperties_EggLayer? EggLayerCompProperties { get; }
}

public interface IMilkableDefTableRecord :
    IThingDefTableRecord
{
    CompProperties_Milkable? MilkableCompProperties { get; }
}

public interface IShearableDefTableRecord :
    IThingDefTableRecord
{
    CompProperties_Shearable? ShearableCompProperties { get; }
}

public interface IRefuelableDefTableRecord :
    IThingDefTableRecord
{
    CompProperties_Refuelable? RefuelableCompProperties { get; }
}

public interface IPowerTraderDefTableRecord :
    IThingDefTableRecord
{
    CompProperties_Power? PowerCompProperties { get; }
}

public interface IThingTableRecord :
    IThingDefTableRecord
{
    Thing Thing { get; }
}

public interface IThingWithCompsTableRecord :
    IThingTableRecord
{
    ThingWithComps ThingWithComps { get; }
}

public interface IPawnTableRecord :
    IThingWithCompsTableRecord,
    IPawnDefTableRecord
{
    Pawn Pawn { get; }
}

public interface IPlantTableRecord :
    IThingWithCompsTableRecord,
    IPlantDefTableRecord
{
    Plant Plant { get; }
}

public interface IBuildingTableRecord :
    IThingWithCompsTableRecord,
    IBuildingDefTableRecord
{
    Building Building { get; }
}

public interface IApparelTableRecord :
    IThingWithCompsTableRecord,
    IApparelDefTableRecord
{
    Apparel Apparel { get; }
}
