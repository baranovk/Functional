using System.Diagnostics.CodeAnalysis;
using Unit = System.ValueTuple;

namespace Functional;

using static F;

public static partial class F
{
    // wrap the given value into a Some
    public static Option<T> Some<T>([NotNull] T? value) // NotNull: `value` is guaranteed to never be null if the method returns without throwing an exception
       => new(value ?? throw new ArgumentNullException(nameof(value)));

    // the None value
    public static NoneType None => default;
}

// a NoneType can be implicitely converted to an Option<T> for any type T
public struct NoneType { }

public readonly struct Option<T> : IEquatable<NoneType>, IEquatable<Option<T>>
{
    private readonly T? _value;
    private readonly bool _isSome;
    private readonly bool _isNone;

    internal Option(T t) => (_isSome, _isNone, _value) = (true, false, t);

    public static implicit operator Option<T>(NoneType _) => default;

    public static implicit operator Option<T>(T t)
       => t is null ? None : new Option<T>(t);

    public TR Match<TR>(Func<TR> None, Func<T, TR> Some) => _isSome ? Some(_value!) : None();

    public IEnumerable<T> AsEnumerable()
    {
        if (_isSome) { yield return _value!; }
    }

    public static bool operator true(Option<T> @this) => @this._isSome;

    public static bool operator false(Option<T> @this) => @this._isNone;

    public static Option<T> operator |(Option<T> l, Option<T> r) => l._isSome ? l : r;

    // equality operators

    public bool Equals(Option<T> other) => _isSome == other._isSome && (_isNone || _value!.Equals(other._value));

    public bool Equals(NoneType other) => _isNone;

    public override bool Equals(object? obj) => obj is Option<T> option && Equals(option);

    public static bool operator ==(Option<T> @this, Option<T> other) => @this.Equals(other);

    public static bool operator !=(Option<T> @this, Option<T> other) => !(@this == other);

    public override int GetHashCode() => _isNone ? 0 : _value!.GetHashCode();

    public override string ToString() => _isSome ? $"Some({_value})" : "None";
}

public static class OptionExt
{
    public static Option<TR> Apply<T, TR>
       (this Option<Func<T, TR>> @this, Option<T> arg)
       => @this.Match(
          () => None,
          (func) => arg.Match(
             () => None,
             (val) => Some(func(val))));

    public static Option<Func<T2, TR>> Apply<T1, T2, TR>
       (this Option<Func<T1, T2, TR>> @this, Option<T1> arg)
       => Apply(@this.Map(F.Curry), arg);

    public static Option<Func<T2, T3, TR>> Apply<T1, T2, T3, TR>
       (this Option<Func<T1, T2, T3, TR>> @this, Option<T1> arg)
       => Apply(@this.Map(F.CurryFirst), arg);

    public static Option<Func<T2, T3, T4, TR>> Apply<T1, T2, T3, T4, TR>
       (this Option<Func<T1, T2, T3, T4, TR>> @this, Option<T1> arg)
       => Apply(@this.Map(F.CurryFirst), arg);

    public static Option<Func<T2, T3, T4, T5, TR>> Apply<T1, T2, T3, T4, T5, TR>
       (this Option<Func<T1, T2, T3, T4, T5, TR>> @this, Option<T1> arg)
       => Apply(@this.Map(F.CurryFirst), arg);

    public static Option<Func<T2, T3, T4, T5, T6, TR>> Apply<T1, T2, T3, T4, T5, T6, TR>
       (this Option<Func<T1, T2, T3, T4, T5, T6, TR>> @this, Option<T1> arg)
       => Apply(@this.Map(F.CurryFirst), arg);

    public static Option<Func<T2, T3, T4, T5, T6, T7, TR>> Apply<T1, T2, T3, T4, T5, T6, T7, TR>
       (this Option<Func<T1, T2, T3, T4, T5, T6, T7, TR>> @this, Option<T1> arg)
       => Apply(@this.Map(F.CurryFirst), arg);

    public static Option<Func<T2, T3, T4, T5, T6, T7, T8, TR>> Apply<T1, T2, T3, T4, T5, T6, T7, T8, TR>
       (this Option<Func<T1, T2, T3, T4, T5, T6, T7, T8, TR>> @this, Option<T1> arg)
       => Apply(@this.Map(F.CurryFirst), arg);

    public static Option<Func<T2, T3, T4, T5, T6, T7, T8, T9, TR>> Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, TR>
       (this Option<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TR>> @this, Option<T1> arg)
       => Apply(@this.Map(F.CurryFirst), arg);

    public static Option<TR> Bind<T, TR>
       (this Option<T> optT, Func<T, Option<TR>> f)
        => optT.Match(
           () => None,
           (t) => f(t));

    public static IEnumerable<TR> Bind<T, TR>
       (this Option<T> @this, Func<T, IEnumerable<TR>> func)
        => @this.AsEnumerable().Bind(func);

    public static Option<Unit> ForEach<T>(this Option<T> @this, Action<T> action)
       => Map(@this, action.ToFunc());

    public static Option<TR> Map<T, TR>
       (this NoneType _, Func<T, TR> f)
       => None;

    public static Option<TR> Map<T, TR>
       (this Option<T> optT, Func<T, TR> f)
       => optT.Match(
          () => None,
          (t) => Some(f(t)));

    public static Option<Func<T2, TR>> Map<T1, T2, TR>
       (this Option<T1> @this, Func<T1, T2, TR> func)
        => @this.Map(func.Curry());

    public static Option<Func<T2, T3, TR>> Map<T1, T2, T3, TR>
       (this Option<T1> @this, Func<T1, T2, T3, TR> func)
        => @this.Map(func.CurryFirst());

    public static IEnumerable<Option<TR>> Traverse<T, TR>(this Option<T> @this, Func<T, IEnumerable<TR>> func)
       => @this.Match(
          () => List((Option<TR>)None),
          (t) => func(t).Map(r => Some(r)));

    // utilities

    public static Unit Match<T>(this Option<T> @this, Action None, Action<T> Some)
        => @this.Match(None.ToFunc(), Some.ToFunc());

    internal static bool IsSome<T>(this Option<T> @this)
       => @this.Match(
          () => false,
          (_) => true);

    internal static T ValueUnsafe<T>(this Option<T> @this)
       => @this.Match(
          () => { throw new InvalidOperationException(); },
          (t) => t);

    public static T GetOrElse<T>(this Option<T> opt, T defaultValue)
       => opt.Match(
          () => defaultValue,
          (t) => t);

    public static T GetOrElse<T>(this Option<T> opt, Func<T> fallback)
       => opt.Match(
          () => fallback(),
          (t) => t);

    public static Task<T> GetOrElse<T>(this Option<T> opt, Func<Task<T>> fallback)
       => opt.Match(
          () => fallback(),
          (t) => Async(t));

    public static Option<T> OrElse<T>(this Option<T> left, Option<T> right)
       => left.Match(
          () => right,
          (_) => left);

    public static Option<T> OrElse<T>(this Option<T> left, Func<Option<T>> right)
       => left.Match(
          () => right(),
          (_) => left);

    public static Validation<T> ToValidation<T>(this Option<T> opt, ValidationError error)
       => opt.Match(
          () => Invalid(error),
          (t) => Valid(t));

    public static Validation<T> ToValidation<T>(this Option<T> opt, Func<ValidationError> error)
       => opt.Match(
          () => Invalid(error()),
          (t) => Valid(t));

    // LINQ

    public static Option<TR> Select<T, TR>(this Option<T> @this, Func<T, TR> func)
       => @this.Map(func);

    public static Option<T> Where<T>
       (this Option<T> optT, Func<T, bool> predicate)
       => optT.Match(
          () => None,
          (t) => predicate(t) ? optT : None);

    public static Option<TRR> SelectMany<T, TR, TRR>
       (this Option<T> opt, Func<T, Option<TR>> bind, Func<T, TR, TRR> project)
       => opt.Match(
          () => None,
          (t) => bind(t).Match(
             () => None,
             (r) => Some(project(t, r))));
}
