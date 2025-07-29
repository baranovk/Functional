using System.Threading;
using Unit = System.ValueTuple;

namespace Functional;

public delegate Exceptional<T> Try<T>();
public delegate Task<Exceptional<T>> AsyncTry<T>(CancellationToken cancellationToken);
public delegate Task<Exceptional<Unit>> AsyncTry(CancellationToken cancellationToken);

public static partial class F
{
    public static Try<T> Try<T>(Func<T> f) => () => f();

    public static AsyncTry<T> TryAsync<T>(Func<CancellationToken, Task<T>> f) => async ct => await f(ct).ConfigureAwait(false);

    public static AsyncTry TryAsync(Func<CancellationToken, Task> f) => async ct => { await f(ct).ConfigureAwait(false); return new Unit(); };
}

public static class TryExt
{
    public static Exceptional<T> Run<T>(this Try<T> @try)
    {
        try { return @try(); }
        catch (Exception ex) { return ex; }
    }

    public static async Task<Exceptional<T>> RunAsync<T>(this AsyncTry<T> @try, CancellationToken cancellationToken = default)
    {
        try { return await @try(cancellationToken).ConfigureAwait(false); }
        catch (Exception ex) { return ex; }
    }

    public static async Task<Exceptional<Unit>> RunAsync(this AsyncTry @try, CancellationToken cancellationToken = default)
    {
        try { return await @try(cancellationToken).ConfigureAwait(false); }
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

    public static AsyncTry Bind
       (this AsyncTry @try, Func<Exceptional<Unit>, CancellationToken, AsyncTry> f)
       => async ct
       => await @try.RunAsync(ct).Bind(t => f(t, ct).RunAsync(ct)).ConfigureAwait(false);

    public static AsyncTry<TR> Bind<T, TR>
       (this AsyncTry<T> @try, Func<Exceptional<T>, CancellationToken, AsyncTry<TR>> f)
       => async ct
       => await @try.RunAsync(ct).Bind(t => f(t, ct).RunAsync(ct)).ConfigureAwait(false);

    public static AsyncTry<TR> MapAsync<T, TR>
       (this AsyncTry<T> @try, Func<Exceptional<T>, CancellationToken, Task<TR>> f, CancellationToken cancellationToken = default)
       => async ct
       => await @try.RunAsync(ct).Bind(t => f(t, ct)).ConfigureAwait(false);

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
