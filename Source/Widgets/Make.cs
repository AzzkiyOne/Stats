using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Stats.Widgets;

public static class Make
{
    #region Basic filters
    public static FilterWidget<TObject> StringFilter<TObject>(Func<TObject, string> lhs)
    {
        return new StringFilter<TObject>(lhs);
    }
    public static FilterWidget<TObject> NumberFilter<TObject>(Func<TObject, decimal> lhs)
    {
        return new NumberFilter<TObject>(lhs);
    }
    public static FilterWidget<TObject> BooleanFilter<TObject>(Func<TObject, bool> lhs)
    {
        return new BooleanFilter<TObject>(lhs);
    }
    #endregion
    #region One-to-many filters
    public static FilterWidget<TObject> OTMFilter<TObject, TOption>(
        Func<TObject, TOption> lhs,
        IEnumerable<NTMFilterOption<TOption>> options
    )
    {
        return new OTMFilter<TObject, TOption>(lhs, options);
    }
    public static FilterWidget<TObject> OTMFilter<TObject, TOption>(
        Func<TObject, TOption> lhs,
        IEnumerable<TObject> objects
    )
    {
        var options = objects
            .Select(lhs)
            .Distinct()
            .OrderBy(option => option)
            .Select<TOption, NTMFilterOption<TOption>>(
                option => option == null ? new() : new(option, option.ToString())
            );

        return OTMFilter(lhs, options);
    }
    public static FilterWidget<TObject> OTMThingDefFilter<TObject>(
        Func<TObject, ThingDef?> lhs,
        IEnumerable<TObject> objects
    )
    {
        var options = objects
            .Select(lhs)
            .Distinct()
            .OrderBy(thingDef => thingDef?.label)
            .Select<ThingDef?, NTMFilterOption<ThingDef?>>(
                thingDef => thingDef == null
                    ? new()
                    : new(thingDef, thingDef.LabelCap, new ThingIcon(thingDef))
            );

        return OTMFilter(lhs, options);
    }
    public static FilterWidget<TObject> OTMDefFilter<TObject, TOption>(
        Func<TObject, TOption> lhs,
        IEnumerable<TObject> objects
    ) where TOption : Def?
    {
        var options = objects
            .Select(lhs)
            .Distinct()
            .OrderBy(def => def?.label)
            .Select<TOption, NTMFilterOption<TOption>>(
                def => def == null ? new() : new(def, def.LabelCap)
            );

        return OTMFilter(lhs, options);
    }
    #endregion
    #region Many-to-many filters
    public static FilterWidget<TObject> MTMFilter<TObject, TOption>(
        Func<TObject, HashSet<TOption>> lhs,
        IEnumerable<NTMFilterOption<TOption>> options
    )
    {
        return new MTMFilter<TObject, TOption>(lhs, options);
    }
    public static FilterWidget<TObject> MTMThingDefFilter<TObject>(
        Func<TObject, HashSet<ThingDef>> lhs,
        IEnumerable<TObject> objects
    )
    {
        var options = objects
            .SelectMany(lhs)
            .Distinct()
            .OrderBy(thingDef => thingDef.label)
            .Select<ThingDef, NTMFilterOption<ThingDef>>(
                thingDef => thingDef == null
                    ? new()
                    : new(thingDef, thingDef.LabelCap, new ThingIcon(thingDef))
            );

        return MTMFilter(lhs, options);
    }
    public static FilterWidget<TObject> MTMDefFilter<TObject, TOption>(
        Func<TObject, HashSet<TOption>> lhs,
        IEnumerable<TObject> objects
    ) where TOption : Def
    {
        var options = objects
            .SelectMany(lhs)
            .Distinct()
            .OrderBy(def => def.label)
            .Select<TOption, NTMFilterOption<TOption>>(
                def => def == null ? new() : new(def, def.LabelCap)
            );

        return MTMFilter(lhs, options);
    }
    #endregion
    #region Misc filters
    public static FilterWidget<TObject> CompositeFilter<TObject>(List<Widget> filters)
    {
        return new CompositeFilter<TObject>(filters);
    }
    #endregion
}
