using RimWorld;

namespace Stats.ThingTable;

public sealed class BuildingRecreationTypeColumnWorker : DefColumnWorker<ThingAlike, JoyKindDef?>
{
    public BuildingRecreationTypeColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override JoyKindDef? GetValue(ThingAlike thing)
    {
        return thing.Def.building?.joyKind;
    }
}
