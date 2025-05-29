namespace Stats.ThingTable
{
    public sealed class IsPlantDestroyedOnHarvestColumnWorker : BooleanColumnWorker<ThingAlike>
    {
        public IsPlantDestroyedOnHarvestColumnWorker(ColumnDef columndef) : base(columndef)
        {
        }
        protected override bool GetValue(ThingAlike thing)
        {
            return thing.Def.plant?.HarvestDestroys == true;
        }
    }
}
