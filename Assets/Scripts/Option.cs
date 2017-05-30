using System;
using Assets.Scripts.Opt;
namespace Assets.Scripts
{
    public struct Unit
    {
    }

    public static class Option
    {
        public static Option<A> some<A>(A value) { return new Some<A>(value); }
        public static None none() { return None.Default; }
    }

    public struct Option<A> : IEquatable<None>, IEquatable<Option<A>>
    {
        readonly A value;
        readonly bool isSome;
        bool isNone => !isSome;

        Option(A value) {
            isSome = true;
            this.value = value;
        }

        public bool Equals(None _) { return isNone; }

        public bool Equals(Option<A> other) { return isSome == other.isSome && (isNone || value.Equals(other.value)); }

        public static implicit operator Option<A>(Some<A> some) { return new Option<A>(some.value); }

        public static implicit operator Option<A>(None _) { return new Option<A>(); }

        public R match<R>(Func<R> none, Func<A, R> some) { return isSome ? some(value) : none(); }

        public R fold<R>(Action none, Func<A, R> some) {
            if (isSome) return some(value);
            else none();
            throw new ArgumentException();
        }

    }

    namespace Opt
    {
        public struct None
        {
            internal static readonly None Default = new None();
        }

        public struct Some<A>
        {
            internal A value { get; }

            internal Some(A value) {
                if (value == null) throw new ArgumentException();
                this.value = value;
            }
        }
    }
}