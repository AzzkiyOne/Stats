namespace Stats.Table.Columns;

public class Column_Label : Column
{
    public override ICell? GetCell(ThingAlike thing)
    {
        return new Cells.Cell_DefRef(
            thing.Stuff == null
                ? thing.Def.LabelCap.ToString()
                : $"{thing.Def.LabelCap} ({thing.Stuff.LabelCap})",
            thing.Def,
            thing.Stuff
        );
    }
}
