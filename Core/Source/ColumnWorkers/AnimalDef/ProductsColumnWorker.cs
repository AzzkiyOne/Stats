using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.TableRecords;
using Stats.TableWorkers;
using Stats.Utils.Extensions;
using Verse;

namespace Stats.ColumnWorkers.AnimalDef;

public sealed class ProductsColumnWorker<TRecord>(ColumnDef columnDef) :
    ThingDefSetColumnWorker<TRecord, ThingDefSetCell>(columnDef)
        where TRecord :
            IMilkableDefTableRecord,
            IEggLayerDefTableRecord,
            IShearableDefTableRecord
{
    protected override ThingDefSetCell MakeCell(TRecord record)
    {
        HashSet<Verse.ThingDef> products = GetProducts(record);

        if (products.Count > 0)
        {
            return new ThingDefSetCell(products);
        }

        return default;
    }

    private static HashSet<Verse.ThingDef> GetProducts(TRecord record)
    {
        CompProperties_Milkable? milkableCompProps = record.MilkableCompProperties;
        CompProperties_EggLayer? eggLayerCompProps = record.EggLayerCompProperties;
        CompProperties_Shearable? shearableCompProps = record.ShearableCompProperties;

        return GetProducts(milkableCompProps, eggLayerCompProps, shearableCompProps);
    }

    private static HashSet<Verse.ThingDef> GetProducts(Verse.ThingDef thingDef)
    {
        CompProperties_Milkable? milkableCompProps = null;
        CompProperties_EggLayer? eggLayerCompProps = null;
        CompProperties_Shearable? shearableCompProps = null;
        foreach (CompProperties compProperties in thingDef.comps)
        {
            if (compProperties is CompProperties_Milkable compProperties_Milkable)
            {
                milkableCompProps = compProperties_Milkable;
            }
            else if (compProperties is CompProperties_EggLayer compProperties_EggLayer)
            {
                eggLayerCompProps = compProperties_EggLayer;
            }
            else if (compProperties is CompProperties_Shearable compProperties_Shearable)
            {
                shearableCompProps = compProperties_Shearable;
            }
        }

        return GetProducts(milkableCompProps, eggLayerCompProps, shearableCompProps);
    }

    private static HashSet<Verse.ThingDef> GetProducts(
        CompProperties_Milkable? milkableCompProps,
        CompProperties_EggLayer? eggLayerCompProps,
        CompProperties_Shearable? shearableCompProps)
    {
        HashSet<Verse.ThingDef> products = new(3);

        if (milkableCompProps != null)
        {
            products.Add(milkableCompProps.milkDef);
        }

        if (eggLayerCompProps != null)
        {
            Verse.ThingDef eggDef = eggLayerCompProps.GetAnyEggDef();

            products.Add(eggDef);
        }

        if (shearableCompProps != null)
        {
            products.Add(shearableCompProps.woolDef);
        }

        return products;
    }

    protected override IEnumerable<Verse.ThingDef?> GetValueFieldFilterOptions(TableWorker tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .SelectMany(GetProducts)
            .Distinct();
    }
}
