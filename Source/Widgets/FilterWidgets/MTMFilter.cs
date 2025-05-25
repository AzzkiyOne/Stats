using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Stats.Widgets;

internal sealed class MTMFilter<TObject, TOption> : NTMFilter<TObject, HashSet<TOption>, TOption>
{
    public MTMFilter(
        Func<TObject, HashSet<TOption>> lhs,
        IEnumerable<NTMFilterOption<TOption>> options
    ) : base(lhs, options, [
        Operators.IntersectsWith.Instance,
        Operators.NotIntersectsWith.Instance,
        Operators.IsSubsetOf.Instance,
        //Operators.IsNotSubsetOf.Instance,
        Operators.IsSupersetOf.Instance,
        //Operators.IsNotSupersetOf.Instance,
        Operators.IsEqualTo.Instance,
        Operators.IsNotEqualTo.Instance,
    ])
    {
    }

    // Reasons for using "! + operator" instead of proper negated set operator symbols:
    // - It looks like there is no negated form for "∩".
    // - Because of font the game uses, "⊄"/"⊅" are unrecognizable in the game.
    private static class Operators
    {
        public sealed class IsEqualTo : AbsOperator
        {
            private IsEqualTo() : base("==", "Is equal to selected") { }
            public override bool Eval(HashSet<TOption> lhs, HashSet<TOption> rhs) => lhs.SetEquals(rhs);
            public static IsEqualTo Instance { get; } = new();
        }

        public sealed class IsNotEqualTo : AbsOperator
        {
            private IsNotEqualTo() : base("!=", "Is not equal to selected") { }
            public override bool Eval(HashSet<TOption> lhs, HashSet<TOption> rhs) => lhs.SetEquals(rhs) == false;
            public static IsNotEqualTo Instance { get; } = new();
        }

        public sealed class IntersectsWith : AbsOperator
        {
            private IntersectsWith() : base("∩", "Contains at least one of selected") { }
            public override bool Eval(HashSet<TOption> lhs, HashSet<TOption> rhs) => rhs.Any(lhs.Contains);
            public static IntersectsWith Instance { get; } = new();
        }

        public sealed class NotIntersectsWith : AbsOperator
        {
            private NotIntersectsWith() : base("!∩", "Does not contain any of selected") { }
            public override bool Eval(HashSet<TOption> lhs, HashSet<TOption> rhs) => rhs.Any(lhs.Contains) == false;
            public static NotIntersectsWith Instance { get; } = new();
        }

        public sealed class IsSubsetOf : AbsOperator
        {
            private IsSubsetOf() : base("⊆", "Is subset of selected") { }
            public override bool Eval(HashSet<TOption> lhs, HashSet<TOption> rhs) => lhs.IsSubsetOf(rhs);
            public static IsSubsetOf Instance { get; } = new();
        }

        // ⊄
        //public sealed class IsNotSubsetOf : GenericOperator
        //{
        //    private IsNotSubsetOf() : base("!⊆", "") { }
        //    public override bool Eval(HashSet<TOption> lhs, HashSet<TOption> rhs) => lhs.IsSubsetOf(rhs) == false;
        //    public static IsNotSubsetOf Instance { get; } = new();
        //}

        public sealed class IsSupersetOf : AbsOperator
        {
            private IsSupersetOf() : base("⊇", "Is superset of selected") { }
            public override bool Eval(HashSet<TOption> lhs, HashSet<TOption> rhs) => lhs.IsSupersetOf(rhs);
            public static IsSupersetOf Instance { get; } = new();
        }

        // ⊅
        //public sealed class IsNotSupersetOf : GenericOperator
        //{
        //    private IsNotSupersetOf() : base("!⊇", "") { }
        //    public override bool Eval(HashSet<TOption> lhs, HashSet<TOption> rhs) => lhs.IsSupersetOf(rhs) == false;
        //    public static IsNotSupersetOf Instance { get; } = new();
        //}
    }
}
