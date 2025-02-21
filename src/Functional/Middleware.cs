namespace Functional;

public delegate dynamic Middleware<T>(Func<T, dynamic> cont);

public static class Middleware
{
    public static T Run<T>(this Middleware<T> mw) => mw(t => t!);

    public static Middleware<TR> Map<T, TR>
       (this Middleware<T> mw, Func<T, TR> f)
       => Select(mw, f);

    public static Middleware<TR> Bind<T, TR>
       (this Middleware<T> mw, Func<T, Middleware<TR>> f)
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
}
