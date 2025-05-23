﻿using Verse;

namespace Stats.Compat.Biotech.GeneTable;

public sealed class ContentSourceColumnWorker : ContentSourceColumnWorker<GeneDef>
{
    public ContentSourceColumnWorker(ColumnDef columnDef) : base(columnDef, false)
    {
    }
    protected override ModContentPack? GetModContentPack(GeneDef geneDef)
    {
        return geneDef.modContentPack;
    }
}
