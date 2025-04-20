using System.Reflection;
using Verse;

namespace Stats.Widgets.Misc;

internal static class DefInfoDialogWidget
{
    private static readonly FieldInfo DialogInfoCardStuffField =
        typeof(Dialog_InfoCard)
        .GetField("stuff", BindingFlags.Instance | BindingFlags.NonPublic);
    public static void Draw(Def def, ThingDef? stuff = null)
    {
        var dialog = new Dialog_InfoCard(def);

        if (stuff != null)
        {
            DialogInfoCardStuffField.SetValue(dialog, stuff);
        }

        Find.WindowStack.Add(dialog);
    }
}
