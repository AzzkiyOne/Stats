namespace Stats.Table.Columns;

public class Column_Label : Column
{
    public override ICell? GetCell(ThingAlike thing)
    {
        var value = thing.Stuff == null
            ? thing.Def.LabelCap.ToString()
            : $"{thing.Def.LabelCap} ({thing.Stuff.LabelCap})";

        return new Cells.Cell_Gen<string>(
            value,
            value,
            thing.Def.description,
            new ThingIcon(thing.Def, thing.Stuff),
            thing
        );
    }
}
