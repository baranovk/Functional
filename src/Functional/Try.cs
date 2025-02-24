namespace Functional;

public delegate Exceptional<T> Try<T>();

public static partial class F
{
    public static Try<T> Try<T>(Func<T> f) => () => f();
}

public static class TryExt
{
    public static Exceptional<T> Run<T>(this Try<T> @try)
    {
        try { return @try(); }
        catch (Exception ex) { return ex; }
    }

    public static Try<TR> Map<T, TR>
       (this Try<T> @try, Func<T, TR> f)
       => ()
       => @try.Run()
             .Match<Exceptional<TR>>(
                ex => ex,
                t => f(t));

    public static Try<Func<T2, TR>> Map<T1, T2, TR>
       (this Try<T1> @try, Func<T1, T2, TR> func)
       => @try.Map(func.Curry());

    public static Try<TR> Bind<T, TR>
       (this Try<T> @try, Func<T, Try<TR>> f)
       => ()
       => @try.Run().Match
          (
             Exception: ex => ex,
             Success: t => f(t).Run()
          );

    // LINQ

    public static Try<TR> Select<T, TR>(this Try<T> @this, Func<T, TR> func) => @this.Map(func);

    public static Try<TRR> SelectMany<T, TR, TRR>
       (this Try<T> @try, Func<T, Try<TR>> bind, Func<T, TR, TRR> project)
       => ()
       => @try.Run().Match(
             ex => ex,
             t => bind(t).Run()
                      .Match<Exceptional<TRR>>(
                         ex => ex,
                         r => project(t, r))
                      );
}
