using Verse;

namespace Stats;

public class ColumnWorker_Name :
    ColumnWorker_Str
{
    public override string? GetValue(ThingRec thing)
    {
        return thing.StuffDef == null
            ? thing.Def.LabelCap
            : $"{thing.Def.LabelCap} ({thing.StuffDef.LabelCap})";
    }
    protected override Widget GetTableCellContent(string? value, ThingRec thing)
    {
        var icon = new Widget_Icon_Thing(thing)
        {
            Width = Text.LineHeight,
            Height = Text.LineHeight,
            Background = (borderBox, _) =>
            {
                Widgets.DrawHighlightIfMouseover(borderBox);

                if (Widgets.ButtonInvisible(borderBox))
                {
                    Widget_DefInfoDialog.Draw(thing.Def, thing.StuffDef);
                }
            }
        };
        var label = new Widget_Label(value!)
        {
            Width = new Widget.Units.Expr(v => v - icon.Width!.Get(v) - 10f),
        };

        return new Widget_Container_Hor([icon, label])
        {
            Width = 100,
            Tooltip = thing.Def.description,
            Gap = 10f,
        };
    }
}
