using System;

namespace OptionLibrary
{
    public class OptionException : Exception
    {
        public OptionException()
        {
        }

        public OptionException(string message)
            : base(message)
        {
        }

        public OptionException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    // Абстракция наличия или отсутствия значения
    public class Option<T>
    {
        public bool IsSome { get; }
        public bool IsNone => !IsSome;

        public T Value
        {
            get
            {
                if (IsNone)
                {
                    throw new OptionException("None object, cannot return Value()");
                }
                return _obj;
            }
        }

        private static readonly Option<T> NoneObject = new Option<T>();
        private readonly T _obj;

        public static Option<T> Some(T value) => new Option<T>(value);

        public static Option<T> None() => NoneObject;

        public Option<TU> Map<TU>(Func<T, TU> f) => IsNone ? Option<TU>.None() : Option<TU>.Some(f(Value));

        public static Option<T> Flatten(Option<Option<T>> option) => option.IsNone ? None() : option.Value;

        public override bool Equals(object rhs)
        {
            Option<T> rhsOption = rhs as Option<T>;
            if (rhsOption == null)
            {
                return false;
            }
            if (IsNone || rhsOption.IsNone)
            {
                return IsNone == rhsOption.IsNone;
            }
            return Equals(Value, rhsOption.Value);
        }

        public override int GetHashCode() => Value == null ? 0 : Value.GetHashCode();

        public static bool operator ==(Option<T> a, Option<T> b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if ((object) a == null || (object) b == null)
            {
                return false;
            }

            return Equals(a.Value, b.Value);
        }

        public static bool operator !=(Option<T> a, Option<T> b)
        {
            return !(a == b);
        }

        private Option()
        {
            IsSome = false;
        }

        private Option(T value)
        {
            _obj = value;
            IsSome = true;
        }
    }
}