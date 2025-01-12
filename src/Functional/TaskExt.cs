using Unit = System.ValueTuple;

namespace Functional;

using static F;

public static partial class F
{
    public static Task<T> Async<T>(T t) => Task.FromResult(t);
}

public static class TaskExt
{
    public static async Task<TR> Apply<T, TR>
       (this Task<Func<T, TR>> f, Task<T> arg)
       //=> (await f)(await arg); // simple version, less efficient
       => (await f.ConfigureAwait(false))(await arg.ConfigureAwait(false)); // ConfigureAwait(false) more efficient, but not for UI-thread apps

    public static Task<Func<T2, TR>> Apply<T1, T2, TR>
       (this Task<Func<T1, T2, TR>> f, Task<T1> arg)
       => Apply(f.Map(F.Curry), arg);

    public static Task<Func<T2, T3, TR>> Apply<T1, T2, T3, TR>
       (this Task<Func<T1, T2, T3, TR>> @this, Task<T1> arg)
       => Apply(@this.Map(F.CurryFirst), arg);

    public static Task<Func<T2, T3, T4, TR>> Apply<T1, T2, T3, T4, TR>
       (this Task<Func<T1, T2, T3, T4, TR>> @this, Task<T1> arg)
       => Apply(@this.Map(F.CurryFirst), arg);

    public static Task<Func<T2, T3, T4, T5, TR>> Apply<T1, T2, T3, T4, T5, TR>
       (this Task<Func<T1, T2, T3, T4, T5, TR>> @this, Task<T1> arg)
       => Apply(@this.Map(F.CurryFirst), arg);

    public static Task<Func<T2, T3, T4, T5, T6, TR>> Apply<T1, T2, T3, T4, T5, T6, TR>
       (this Task<Func<T1, T2, T3, T4, T5, T6, TR>> @this, Task<T1> arg)
       => Apply(@this.Map(F.CurryFirst), arg);

    public static Task<Func<T2, T3, T4, T5, T6, T7, TR>> Apply<T1, T2, T3, T4, T5, T6, T7, TR>
       (this Task<Func<T1, T2, T3, T4, T5, T6, T7, TR>> @this, Task<T1> arg)
       => Apply(@this.Map(F.CurryFirst), arg);

    public static Task<Func<T2, T3, T4, T5, T6, T7, T8, TR>> Apply<T1, T2, T3, T4, T5, T6, T7, T8, TR>
       (this Task<Func<T1, T2, T3, T4, T5, T6, T7, T8, TR>> @this, Task<T1> arg)
       => Apply(@this.Map(F.CurryFirst), arg);

    public static Task<Func<T2, T3, T4, T5, T6, T7, T8, T9, TR>> Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, TR>
       (this Task<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TR>> @this, Task<T1> arg)
       => Apply(@this.Map(F.CurryFirst), arg);

    public static async Task<TR> Map<T, TR>
       (this Task<T> task, Func<T, TR> f)
       //=> f(await task);
       => f(await task.ConfigureAwait(false));

    public static async Task<TR> Map<TR>
       (this Task task, Func<TR> f)
    {
        await task.ConfigureAwait(false);
        return f();
    }

    public static Task<Func<T2, TR>> Map<T1, T2, TR>
       (this Task<T1> @this, Func<T1, T2, TR> func)
        => @this.Map(func.Curry());

    public static Task<Func<T2, T3, TR>> Map<T1, T2, T3, TR>
       (this Task<T1> @this, Func<T1, T2, T3, TR> func)
        => @this.Map(func.CurryFirst());

    public static Task<Func<T2, T3, T4, TR>> Map<T1, T2, T3, T4, TR>
       (this Task<T1> @this, Func<T1, T2, T3, T4, TR> func)
        => @this.Map(func.CurryFirst());

    public static Task<Func<T2, T3, T4, T5, TR>> Map<T1, T2, T3, T4, T5, TR>
       (this Task<T1> @this, Func<T1, T2, T3, T4, T5, TR> func)
        => @this.Map(func.CurryFirst());

    public static Task<Func<T2, T3, T4, T5, T6, TR>> Map<T1, T2, T3, T4, T5, T6, TR>
       (this Task<T1> @this, Func<T1, T2, T3, T4, T5, T6, TR> func)
        => @this.Map(func.CurryFirst());

    public static Task<Func<T2, T3, T4, T5, T6, T7, TR>> Map<T1, T2, T3, T4, T5, T6, T7, TR>
       (this Task<T1> @this, Func<T1, T2, T3, T4, T5, T6, T7, TR> func)
        => @this.Map(func.CurryFirst());

    public static Task<Func<T2, T3, T4, T5, T6, T7, T8, TR>> Map<T1, T2, T3, T4, T5, T6, T7, T8, TR>
       (this Task<T1> @this, Func<T1, T2, T3, T4, T5, T6, T7, T8, TR> func)
        => @this.Map(func.CurryFirst());

    public static Task<Func<T2, T3, T4, T5, T6, T7, T8, T9, TR>> Map<T1, T2, T3, T4, T5, T6, T7, T8, T9, TR>
       (this Task<T1> @this, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TR> func)
        => @this.Map(func.CurryFirst());

    public static Task<TR> Map<T, TR>
       (this Task<T> task, Func<Exception, TR> Faulted, Func<T, TR> Completed)
       => task.ContinueWith(t =>
             t.Status == TaskStatus.Faulted
                ? Faulted(t.Exception!)
                : Completed(t.Result), TaskScheduler.Default);

    public static Task<Unit> ForEach<T>(this Task<T> @this, Action<T> continuation, CancellationToken cancellationToken = default)
        => @this.ContinueWith(t => continuation.ToFunc()(t.Result),
            cancellationToken, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);

    public static async Task<TR> Bind<T, TR>(this Task<T> task, Func<T, Task<TR>> f)
        => await f(await task.ConfigureAwait(false)).ConfigureAwait(false);

    public static Task<T> OrElse<T>
       (this Task<T> task, Func<Task<T>> fallback, CancellationToken cancellationToken = default)
       => task.ContinueWith(t =>
             t.Status == TaskStatus.Faulted
                ? fallback()
                : Task.FromResult(t.Result), cancellationToken, TaskContinuationOptions.None, TaskScheduler.Default
          )
          .Unwrap();

    public static Task<T> Recover<T>
       (this Task<T> task, Func<Exception, T> fallback)
       => task.ContinueWith(t =>
             t.Status == TaskStatus.Faulted
                ? fallback(t.Exception!)
                : t.Result, TaskScheduler.Default);

    public static Task<T> RecoverWith<T>
       (this Task<T> task, Func<Exception, Task<T>> fallback)
       => task.ContinueWith(t =>
             t.Status == TaskStatus.Faulted
                ? fallback(t.Exception!)
                : Task.FromResult(t.Result), TaskScheduler.Default
       ).Unwrap();

    // LINQ

    public static async Task<TRR> SelectMany<T, TR, TRR>
       (this Task<T> task, Func<T, Task<TR>> bind, Func<T, TR, TRR> project)
    {
        var t = await task.ConfigureAwait(false);
        var r = await bind(t).ConfigureAwait(false);
        return project(t, r);
    }

    public static async Task<TRR> SelectMany<T, TR, TRR>
       (this Task<T> task, Func<T, ValueTask<TR>> bind, Func<T, TR, TRR> project)
    {
        var t = await task.ConfigureAwait(false);
        var r = await bind(t).ConfigureAwait(false);
        return project(t, r);
    }

    public static async Task<TRR> SelectMany<TR, TRR>
       (this Task task, Func<Unit, Task<TR>> bind, Func<Unit, TR, TRR> project)
    {
        await task.ConfigureAwait(false);
        var r = await bind(Unit()).ConfigureAwait(false);
        return project(Unit(), r);
    }

    public static async Task<TR> SelectMany<T, TR>(this Task<T> task, Func<T, Task<TR>> f)
       => await f(await task.ConfigureAwait(false)).ConfigureAwait(false);

    public static async Task<TR> Select<T, TR>(this Task<T> task, Func<T, TR> f)
       => f(await task.ConfigureAwait(false));

    public static async Task<T> Where<T>(this Task<T> source, Func<T, bool> predicate)
    {
        var t = await source.ConfigureAwait(false);

        if (!predicate(t))
        {
            throw new OperationCanceledException();
        }

        return t;
    }

    public static async Task<TV> Join<T, TU, TK, TV>(
        this Task<T> source, Task<TU> inner,
        Func<T, TK> outerKeySelector, Func<TU, TK> innerKeySelector,
        Func<T, TU, TV> resultSelector)
    {
        await Task.WhenAll(source, inner).ConfigureAwait(false);

        var sr = await source.ConfigureAwait(false);
        var ir = await inner.ConfigureAwait(false);

        if (!EqualityComparer<TK>.Default.Equals(outerKeySelector(sr), innerKeySelector(ir)))
        {
            throw new OperationCanceledException();
        }

        return resultSelector(sr, ir);
    }

    public static async Task<TV> GroupJoin<T, TU, TK, TV>(
        this Task<T> source, Task<TU> inner,
        Func<T, TK> outerKeySelector, Func<TU, TK> innerKeySelector,
        Func<T, Task<TU>, TV> resultSelector)
    {
        var t = await source.ConfigureAwait(false);

        return resultSelector(t,
            inner.Where(u => EqualityComparer<TK>.Default.Equals(
                outerKeySelector(t), innerKeySelector(u))));
    }
}
