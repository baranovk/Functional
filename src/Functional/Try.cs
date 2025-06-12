using Unit = System.ValueTuple;

namespace Functional;

public delegate Exceptional<T> Try<T>();
public delegate Task<Exceptional<T>> AsyncTry<T>();
public delegate Task<Exceptional<Unit>> AsyncTry();

public static partial class F
{
    public static Try<T> Try<T>(Func<T> f) => () => f();

    public static AsyncTry<T> TryAsync<T>(Func<Task<T>> f) => async () => await f().ConfigureAwait(false);

    public static AsyncTry TryAsync(Func<Task> f) => async () => { await f().ConfigureAwait(false); return new Unit(); };
}

public static class TryExt
{
    public static Exceptional<T> Run<T>(this Try<T> @try)
    {
        try { return @try(); }
        catch (Exception ex) { return ex; }
    }

    public static async Task<Exceptional<T>> RunAsync<T>(this AsyncTry<T> @try)
    {
        try { return await @try().ConfigureAwait(false); }
        catch (Exception ex) { return ex; }
    }

    public static async Task<Exceptional<Unit>> RunAsync(this AsyncTry @try)
    {
        try { return await @try().ConfigureAwait(false); }
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

    public static AsyncTry<TR> Bind<T, TR>
       (this AsyncTry<T> @try, Func<Exceptional<T>, AsyncTry<TR>> f)
       => async ()
       => await @try.RunAsync().Bind(t => f(t).RunAsync()).ConfigureAwait(false);

    public static AsyncTry<TR> MapAsync<T, TR>
       (this AsyncTry<T> @try, Func<Exceptional<T>, Task<TR>> f)
       => (async ()
       => await @try.RunAsync().Bind(t => f(t)).ConfigureAwait(false));

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
