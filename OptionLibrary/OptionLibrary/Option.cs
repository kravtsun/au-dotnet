using System;

namespace OptionLibrary
{
    // Абстракция наличия или отсутствия значения
    public class Option<T>
    {
        private readonly T _obj;
        public bool IsSome { get; }
        public bool IsNone => !IsSome;

        public static Option<T> Some(T value)
        {
            return new Option<T>(value);
        }

        public static Option<T> None()
        {
            return new Option<T>();
        }

        public T Value()
        {
            if (IsNone)
            {
                throw new InvalidOperationException();
            }
            return _obj;
        }

        public Option<TU> Map<TU>(Func<T, TU> f)
        {
            return IsNone ? Option<TU>.None() : Option<TU>.Some(f(_obj));
        }

        public static Option<T> Flatten(Option<Option<T>> option)
        {
            return option.IsNone ? None() : option.Value();
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
