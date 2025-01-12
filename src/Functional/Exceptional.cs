using Unit = System.ValueTuple;

namespace Functional;

public static partial class F
{
    public static Exceptional<T> Exceptional<T>(T t) => new(t);
}

public readonly struct Exceptional<T>
{
    private Exception? Exception { get; }

    private T? Value { get; }

    private bool IsSuccess { get; }
    private bool IsException => !IsSuccess;

    internal Exceptional(Exception ex)
    {
        IsSuccess = false;
        Exception = ex ?? throw new ArgumentNullException(nameof(ex));
        Value = default;
    }

    internal Exceptional(T value)
    {
        IsSuccess = true;
        Value = value ?? throw new ArgumentNullException(nameof(value));
        Exception = default;
    }

    public static implicit operator Exceptional<T>(Exception ex) => new(ex);
    public static implicit operator Exceptional<T>(T t) => new(t);

    public TR Match<TR>(Func<Exception, TR> Exception, Func<T, TR> Success)
       => IsException ? Exception(this.Exception!) : Success(Value!);

    public Unit Match(Action<Exception> Exception, Action<T> Success)
       => Match(Exception.ToFunc(), Success.ToFunc());

    public override string ToString()
       => Match(
          ex => $"Exception({ex.Message})",
          t => $"Success({t})");
}

public static class Exceptional
{
    // creating a new Exceptional

    public static Func<T, Exceptional<T>> Return<T>()
       => t => t;

    public static Exceptional<TR> Of<TR>(Exception left) => new(left);

    public static Exceptional<TR> Of<TR>(TR right) => new(right);

    // applicative

    public static Exceptional<TR> Apply<T, TR>
       (this Exceptional<Func<T, TR>> @this, Exceptional<T> arg)
       => @this.Match(
          Exception: ex => ex,
          Success: func => arg.Match(
             Exception: ex => ex,
             Success: t => F.Exceptional(func(t))));

    public static Exceptional<Func<T2, TR>> Apply<T1, T2, TR>
       (this Exceptional<Func<T1, T2, TR>> @this, Exceptional<T1> arg)
       => Apply(@this.Map(F.Curry), arg);

    public static Exceptional<Func<T2, T3, TR>> Apply<T1, T2, T3, TR>
       (this Exceptional<Func<T1, T2, T3, TR>> @this, Exceptional<T1> arg)
       => Apply(@this.Map(F.CurryFirst), arg);

    public static Exceptional<Func<T2, T3, T4, TR>> Apply<T1, T2, T3, T4, TR>
       (this Exceptional<Func<T1, T2, T3, T4, TR>> @this, Exceptional<T1> arg)
       => Apply(@this.Map(F.CurryFirst), arg);

    public static Exceptional<Func<T2, T3, T4, T5, TR>> Apply<T1, T2, T3, T4, T5, TR>
       (this Exceptional<Func<T1, T2, T3, T4, T5, TR>> @this, Exceptional<T1> arg)
       => Apply(@this.Map(F.CurryFirst), arg);

    public static Exceptional<Func<T2, T3, T4, T5, T6, TR>> Apply<T1, T2, T3, T4, T5, T6, TR>
       (this Exceptional<Func<T1, T2, T3, T4, T5, T6, TR>> @this, Exceptional<T1> arg)
       => Apply(@this.Map(F.CurryFirst), arg);

    public static Exceptional<Func<T2, T3, T4, T5, T6, T7, TR>> Apply<T1, T2, T3, T4, T5, T6, T7, TR>
       (this Exceptional<Func<T1, T2, T3, T4, T5, T6, T7, TR>> @this, Exceptional<T1> arg)
       => Apply(@this.Map(F.CurryFirst), arg);

    public static Exceptional<Func<T2, T3, T4, T5, T6, T7, T8, TR>> Apply<T1, T2, T3, T4, T5, T6, T7, T8, TR>
       (this Exceptional<Func<T1, T2, T3, T4, T5, T6, T7, T8, TR>> @this, Exceptional<T1> arg)
       => Apply(@this.Map(F.CurryFirst), arg);

    public static Exceptional<Func<T2, T3, T4, T5, T6, T7, T8, T9, TR>> Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, TR>
       (this Exceptional<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TR>> @this, Exceptional<T1> arg)
       => Apply(@this.Map(F.CurryFirst), arg);

    // functor

    public static Exceptional<TRR> Map<TR, TRR>
    (
       this Exceptional<TR> @this,
       Func<TR, TRR> f
    )
    => @this.Match
    (
       Exception: ex => new Exceptional<TRR>(ex),
       Success: r => f(r)
    );

    public static Exceptional<Unit> ForEach<TR>(this Exceptional<TR> @this, Action<TR> act)
       => Map(@this, act.ToFunc());

    public static Exceptional<TRR> Bind<TR, TRR>
    (
       this Exceptional<TR> @this,
       Func<TR, Exceptional<TRR>> f
    )
    => @this.Match
    (
       Exception: ex => new Exceptional<TRR>(ex),
       Success: r => f(r)
    );

    // LINQ

    public static Exceptional<TR> Select<T, TR>(this Exceptional<T> @this, Func<T, TR> map) => @this.Map(map);

    public static Exceptional<TRR> SelectMany<T, TR, TRR>
    (
       this Exceptional<T> @this,
       Func<T, Exceptional<TR>> bind,
       Func<T, TR, TRR> project
    )
    => @this.Match
    (
       Exception: ex => new Exceptional<TRR>(ex),
       Success: t => bind(t).Match
       (
          Exception: ex => new Exceptional<TRR>(ex),
          Success: r => project(t, r)
       )
    );
}
