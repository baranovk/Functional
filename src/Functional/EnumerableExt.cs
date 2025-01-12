using System.Collections.Immutable;
using Unit = System.ValueTuple;

namespace Functional;

using static F;

public static partial class F
{
    public static IEnumerable<T> List<T>(params T[] items) => items.ToImmutableList();
}

public static class EnumerableExt
{
    public static Func<T, IEnumerable<T>> Return<T>() => t => List(t);

    public static IEnumerable<T> Append<T>(this IEnumerable<T> source, params T[] ts) => source.Concat(ts);

    public static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, T val)
    {
        yield return val;

        foreach (T t in source)
        {
            yield return t;
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1851:Possible multiple enumerations of 'IEnumerable' collection", Justification = "<Pending>")]
    public static (IEnumerable<T> Passed, IEnumerable<T> Failed) Partition<T>
    (
       this IEnumerable<T> source,
       Func<T, bool> predicate
    )
    {
        var grouped = source.GroupBy(predicate);

        return
        (
           Passed: grouped.Where(g => g.Key).FirstOrDefault(Enumerable.Empty<T>()),
           Failed: grouped.Where(g => !g.Key).FirstOrDefault(Enumerable.Empty<T>())
        );
    }

    public static Option<T> Find<T>(this IEnumerable<T> source, Func<T, bool> predicate)
       => source.Where(predicate).Head();

    public static IEnumerable<Unit> ForEach<T>
       (this IEnumerable<T> ts, Action<T> action)
       => ts.Map(action.ToFunc()).ToImmutableList();

    public static IEnumerable<TR> MapInTermsOfFold<T, TR>
       (this IEnumerable<T> ts, Func<T, TR> f)
        => ts.Aggregate(List<TR>()
           , (rs, t) => rs.Append(f(t)));

    public static IEnumerable<T> WhereInTermsOfFold<T>
       (this IEnumerable<T> @this, Func<T, bool> predicate)
        => @this.Aggregate(List<T>()
           , (ts, t) => predicate(t) ? ts.Append(t) : ts);

    public static IEnumerable<TR> BindInTermsOfFold<T, TR>
       (this IEnumerable<T> ts, Func<T, IEnumerable<TR>> f)
       => ts.Aggregate(List<TR>()
          , (rs, t) => rs.Concat(f(t)));

    public static TR Match<T, TR>(this IEnumerable<T> list
       , Func<TR> Empty, Func<T, IEnumerable<T>, TR> Otherwise)
       => list.Head().Match(
          None: Empty,
          Some: head => Otherwise(head, list.Skip(1)));

    public static Option<T> Head<T>(this IEnumerable<T> list)
    {
        if (list == null) { return None; }

        var enumerator = list.GetEnumerator();
        return enumerator.MoveNext() ? Some(enumerator.Current) : None;
    }

    public static IEnumerable<Func<T2, TR>> Map<T1, T2, TR>(this IEnumerable<T1> list, Func<T1, T2, TR> func) => list.Map(func.Curry());

    public static IEnumerable<TR> Map<T, TR>(this IEnumerable<T> list, Func<T, TR> func) => list.Select(func);

    public static IEnumerable<Func<T2, Func<T3, TR>>> Map<T1, T2, T3, TR>(this IEnumerable<T1> opt, Func<T1, T2, T3, TR> func)
       => opt.Map(func.Curry());

    public static IEnumerable<TR> Bind<T, TR>(this IEnumerable<T> list, Func<T, IEnumerable<TR>> func)
        => list.SelectMany(func);

    public static IEnumerable<TR> Bind<T, TR>(this IEnumerable<T> list, Func<T, Option<TR>> func)
        => list.Bind(t => func(t).AsEnumerable());

    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> list)
        => list.SelectMany(x => x);

    // LINQ

    public static IEnumerable<TRR> SelectMany<T, TR, TRR>
       (this IEnumerable<T> source,
        Func<T, Option<TR>> bind,
        Func<T, TR, TRR> project)
       => from t in source
          let opt = bind(t)
          where opt.IsSome()
          select project(t, opt.ValueUnsafe());

    public static IEnumerable<T> TakeWhile<T>(this IEnumerable<T> @this, Func<T, bool> pred)
    {
        foreach (var item in @this)
        {
            if (pred(item))
            {
                yield return item;
            }
            else
            {
                yield break;
            }
        }
    }

    public static IEnumerable<T> DropWhile<T>(this IEnumerable<T> @this, Func<T, bool> pred)
    {
        var clean = true;
        foreach (var item in @this)
        {
            if (!clean || !pred(item))
            {
                yield return item;
                clean = false;
            }
        }
    }
}
