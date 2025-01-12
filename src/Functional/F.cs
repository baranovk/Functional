using Unit = System.ValueTuple;

namespace Functional;

public static partial class F
{
    public static Unit Unit() => default;

    // function manipulation 

    public static Func<T1, Func<T2, TR>> Curry<T1, T2, TR>(this Func<T1, T2, TR> func)
        => t1 => t2 => func(t1, t2);

    public static Func<T1, Func<T2, Func<T3, TR>>> Curry<T1, T2, T3, TR>(this Func<T1, T2, T3, TR> func)
        => t1 => t2 => t3 => func(t1, t2, t3);

    public static Func<T1, Func<T2, T3, TR>> CurryFirst<T1, T2, T3, TR>
       (this Func<T1, T2, T3, TR> @this) => t1 => (t2, t3) => @this(t1, t2, t3);

    public static Func<T1, Func<T2, T3, T4, TR>> CurryFirst<T1, T2, T3, T4, TR>
       (this Func<T1, T2, T3, T4, TR> @this) => t1 => (t2, t3, t4) => @this(t1, t2, t3, t4);

    public static Func<T1, Func<T2, T3, T4, T5, TR>> CurryFirst<T1, T2, T3, T4, T5, TR>
       (this Func<T1, T2, T3, T4, T5, TR> @this) => t1 => (t2, t3, t4, t5) => @this(t1, t2, t3, t4, t5);

    public static Func<T1, Func<T2, T3, T4, T5, T6, TR>> CurryFirst<T1, T2, T3, T4, T5, T6, TR>
       (this Func<T1, T2, T3, T4, T5, T6, TR> @this) => t1 => (t2, t3, t4, t5, t6) => @this(t1, t2, t3, t4, t5, t6);

    public static Func<T1, Func<T2, T3, T4, T5, T6, T7, TR>> CurryFirst<T1, T2, T3, T4, T5, T6, T7, TR>
       (this Func<T1, T2, T3, T4, T5, T6, T7, TR> @this) => t1 => (t2, t3, t4, t5, t6, t7) => @this(t1, t2, t3, t4, t5, t6, t7);

    public static Func<T1, Func<T2, T3, T4, T5, T6, T7, T8, TR>> CurryFirst<T1, T2, T3, T4, T5, T6, T7, T8, TR>
       (this Func<T1, T2, T3, T4, T5, T6, T7, T8, TR> @this) => t1 => (t2, t3, t4, t5, t6, t7, t8) => @this(t1, t2, t3, t4, t5, t6, t7, t8);

    public static Func<T1, Func<T2, T3, T4, T5, T6, T7, T8, T9, TR>> CurryFirst<T1, T2, T3, T4, T5, T6, T7, T8, T9, TR>
       (this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TR> @this) => t1 => (t2, t3, t4, t5, t6, t7, t8, t9) => @this(t1, t2, t3, t4, t5, t6, t7, t8, t9);

    public static Func<T, T> Tap<T>(Action<T> act)
       => x => { act(x); return x; };

    public static TR Pipe<T, TR>(this T @this, Func<T, TR> func) => func(@this);

    /// <summary>
    /// Pipes the input value in the given Action, i.e. invokes the given Action on the given value.
    /// returning the input value. Not really a genuine implementation of pipe, since it combines pipe with Tap.
    /// </summary>
    public static T Pipe<T>(this T input, Action<T> func) => Tap(func)(input);

    // Using
    public static TR Using<TDisp, TR>(TDisp disposable
       , Func<TDisp, TR> func) where TDisp : IDisposable
    {
        using var disp = disposable;
        return func(disp);
    }

    public static Unit Using<TDisp>(TDisp disposable
       , Action<TDisp> act) where TDisp : IDisposable
       => Using(disposable, act.ToFunc());

    public static TR Using<TDisp, TR>(Func<TDisp> createDisposable
       , Func<TDisp, TR> func) where TDisp : IDisposable
    {
        using var disp = createDisposable();
        return func(disp);
    }

    public static Unit Using<TDisp>(Func<TDisp> createDisposable
       , Action<TDisp> action) where TDisp : IDisposable
       => Using(createDisposable, action.ToFunc());

    // Range
    public static IEnumerable<char> Range(char from, char to)
    {
        for (var i = from; i <= to; i++)
        {
            yield return i;
        }
    }

    public static IEnumerable<int> Range(int from, int to)
    {
        for (var i = from; i <= to; i++)
        {
            yield return i;
        }
    }
}
