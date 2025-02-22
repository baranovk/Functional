using static Functional.F;

namespace Functional;

public delegate dynamic Middleware<T>(Func<T, dynamic> cont);

public delegate Task<dynamic> AsyncMiddleware<T>(Func<T, Task<dynamic>> cont);

public static class Middleware
{
    public static T Run<T>(this Middleware<T> mw) => mw(t => t!);

    public static async Task<T> RunAsync<T>(this AsyncMiddleware<T> mw) => await mw(t => Async<dynamic>(t!)).ConfigureAwait(false);

    public static Middleware<TR> Map<T, TR>
       (this Middleware<T> mw, Func<T, TR> f)
       => Select(mw, f);

    public static Middleware<TR> Bind<T, TR>
       (this Middleware<T> mw, Func<T, Middleware<TR>> f)
       => SelectMany(mw, f);

    public static AsyncMiddleware<TR> Bind<T, TR>
       (this AsyncMiddleware<T> mw, Func<T, AsyncMiddleware<TR>> f)
       => SelectMany(mw, f);

    public static Middleware<TR> Select<T, TR>
       (this Middleware<T> mw, Func<T, TR> f)
       => cont => mw(t => cont(f(t)));

    public static Middleware<TR> SelectMany<T, TR>
       (this Middleware<T> mw, Func<T, Middleware<TR>> f)
       => cont => mw(t => f(t)(cont));

    public static Middleware<TRR> SelectMany<T, TR, TRR>
       (this Middleware<T> @this, Func<T, Middleware<TR>> f, Func<T, TR, TRR> project)
       => cont => @this(t => f(t)(r => cont(project(t, r))));

    public static AsyncMiddleware<TR> SelectMany<T, TR>
      (this AsyncMiddleware<T> mw, Func<T, AsyncMiddleware<TR>> f)
      => cont => mw(t => f(t)(cont));
}
