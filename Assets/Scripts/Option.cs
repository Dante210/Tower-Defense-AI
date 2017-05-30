using System;
using Assets.Scripts.Opt;
namespace Assets.Scripts
{

    public static class Option
    {
        public static Option<A> some<A>(A value) => new Opt.Some<A>(value);
        public static Opt.None none () => Opt.None.Default;
    }

    public struct Option<A> : IEquatable<None>, IEquatable<Option<A>>
    {
        readonly A value;
        readonly bool isSome;
        bool isNone => !isSome;

        Option(A value) {
            this.isSome = true;
            this.value = value;
        }

        public bool Equals(None _) => isNone;
        public bool Equals(Option<A> other) => isSome == other.isSome && (isNone || value.Equals(other.value));

        public static implicit operator Option<A>(Opt.Some<A> some) => new Option<A>(some.value);
        public static implicit operator Option<A>(Opt.None _) => new Option<A>(); 
        public R match<R>(Func<R> none, Func<A, R> some) => isSome ? some(value) : none();
        public void match(Action none, Action<A> some) {
            if (isSome) some(value);
            else none();
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