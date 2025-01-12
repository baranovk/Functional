using Unit = System.ValueTuple;

namespace Functional;

using static F;

public static partial class F
{
    public static Validation<T> Valid<T>(T value) => new(value ?? throw new ArgumentNullException(nameof(value)));

    public static ValidationError Error(string message) => new(message);

    // create a Validation in the Invalid state
    public static Validation.Invalid Invalid(params ValidationError[] errors) => new(errors);

    public static Validation<T> Invalid<T>(params ValidationError[] errors) => new Validation.Invalid(errors);

    public static Validation.Invalid Invalid(IEnumerable<ValidationError> errors) => new(errors);

    public static Validation<T> Invalid<T>(IEnumerable<ValidationError> errors) => new Validation.Invalid(errors);
}

public record ValidationError(string Message)
{
    public override string ToString() => Message;

    public static implicit operator ValidationError(string m) => new(m);
}

public readonly struct Validation<T> : IEquatable<Validation<T>>
{
    internal IEnumerable<ValidationError> Errors { get; }
    internal T? Value { get; }

    public bool IsValid { get; }

    public static Validation<T> Fail(IEnumerable<ValidationError> errors) => new(errors);

    public static Validation<T> Fail(params ValidationError[] errors) => new(errors.AsEnumerable());

    private Validation(IEnumerable<ValidationError> errors)
       => (IsValid, Errors, Value) = (false, errors, default);

    internal Validation(T t)
       => (IsValid, Errors, Value) = (true, Enumerable.Empty<ValidationError>(), t);

    public TR Match<TR>(Func<IEnumerable<ValidationError>, TR> Invalid, Func<T, TR> Valid)
       => IsValid ? Valid(Value!) : Invalid(Errors);

    public Unit Match(Action<IEnumerable<ValidationError>> Invalid, Action<T> Valid)
       => Match(Invalid.ToFunc(), Valid.ToFunc());

    public IEnumerable<T> AsEnumerable()
    {
        if (IsValid) { yield return Value!; }
    }

    public override string ToString()
       => IsValid
          ? $"Valid({Value})"
          : $"Invalid([{string.Join(", ", Errors)}])";

    public override bool Equals(object? obj)
       => obj is Validation<T> other && Equals(other);

    public bool Equals(Validation<T> other) => IsValid == other.IsValid
          && (IsValid && Value!.Equals(other.Value) || ToString() == other.ToString());

    public override int GetHashCode() => Match
    (
       Invalid: errs => errs.GetHashCode(),
       Valid: t => t!.GetHashCode()
    );

    public static implicit operator Validation<T>(ValidationError error) => new(new[] { error });
    public static implicit operator Validation<T>(Validation.Invalid left) => new(left.Errors);
    public static implicit operator Validation<T>(T right) => Valid(right);

    public static bool operator ==(Validation<T> left, Validation<T> right) => left.Equals(right);

    public static bool operator !=(Validation<T> left, Validation<T> right) => !(left == right);
}

public static class Validation
{
    public struct Invalid
    {
        internal IEnumerable<ValidationError> Errors;
        public Invalid(IEnumerable<ValidationError> errors) { Errors = errors; }
    }

    public static T GetOrElse<T>(this Validation<T> opt, T defaultValue)
       => opt.Match(
          (errs) => defaultValue,
          (t) => t);

    public static T GetOrElse<T>(this Validation<T> opt, Func<T> fallback)
       => opt.Match(
          (errs) => fallback(),
          (t) => t);

    public static Validation<TR> Apply<T, TR>(this Validation<Func<T, TR>> valF, Validation<T> valT)
       => valF.Match(
          Valid: (f) => valT.Match(
             Valid: (t) => Valid(f(t)),
             Invalid: (err) => Invalid(err)),
          Invalid: (errF) => valT.Match(
             Valid: (_) => Invalid(errF),
             Invalid: (errT) => Invalid(errF.Concat(errT))));

    public static Validation<Func<T2, TR>> Apply<T1, T2, TR>
       (this Validation<Func<T1, T2, TR>> @this, Validation<T1> arg)
       => Apply(@this.Map(F.Curry), arg);

    public static Validation<Func<T2, T3, TR>> Apply<T1, T2, T3, TR>
       (this Validation<Func<T1, T2, T3, TR>> @this, Validation<T1> arg)
       => Apply(@this.Map(F.CurryFirst), arg);

    public static Validation<Func<T2, T3, T4, TR>> Apply<T1, T2, T3, T4, TR>
       (this Validation<Func<T1, T2, T3, T4, TR>> @this, Validation<T1> arg)
       => Apply(@this.Map(F.CurryFirst), arg);

    public static Validation<Func<T2, T3, T4, T5, TR>> Apply<T1, T2, T3, T4, T5, TR>
       (this Validation<Func<T1, T2, T3, T4, T5, TR>> @this, Validation<T1> arg)
       => Apply(@this.Map(F.CurryFirst), arg);

    public static Validation<Func<T2, T3, T4, T5, T6, TR>> Apply<T1, T2, T3, T4, T5, T6, TR>
       (this Validation<Func<T1, T2, T3, T4, T5, T6, TR>> @this, Validation<T1> arg)
       => Apply(@this.Map(F.CurryFirst), arg);

    public static Validation<Func<T2, T3, T4, T5, T6, T7, TR>> Apply<T1, T2, T3, T4, T5, T6, T7, TR>
       (this Validation<Func<T1, T2, T3, T4, T5, T6, T7, TR>> @this, Validation<T1> arg)
       => Apply(@this.Map(F.CurryFirst), arg);

    public static Validation<Func<T2, T3, T4, T5, T6, T7, T8, TR>> Apply<T1, T2, T3, T4, T5, T6, T7, T8, TR>
       (this Validation<Func<T1, T2, T3, T4, T5, T6, T7, T8, TR>> @this, Validation<T1> arg)
       => Apply(@this.Map(F.CurryFirst), arg);

    public static Validation<Func<T2, T3, T4, T5, T6, T7, T8, T9, TR>> Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, TR>
       (this Validation<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TR>> @this, Validation<T1> arg)
       => Apply(@this.Map(F.CurryFirst), arg);

    public static Validation<TR> Map<T, TR>
       (this Validation<T> @this, Func<T, TR> f)
       => @this.Match
       (
          Valid: t => Valid(f(t)),
          Invalid: errs => Invalid(errs)
       );

    public static Validation<Func<T2, TR>> Map<T1, T2, TR>(this Validation<T1> @this
       , Func<T1, T2, TR> func)
        => @this.Map(func.Curry());

    public static Validation<Unit> ForEach<TR>
       (this Validation<TR> @this, Action<TR> act)
       => Map(@this, act.ToFunc());

    public static Validation<T> Do<T>
       (this Validation<T> @this, Action<T> action)
    {
        @this.ForEach(action);
        return @this;
    }

    public static Validation<TR> Bind<T, TR>
       (this Validation<T> val, Func<T, Validation<TR>> f)
        => val.Match(
           Invalid: (err) => Invalid(err),
           Valid: (r) => f(r));

    // LINQ

    public static Validation<TR> Select<T, TR>(this Validation<T> @this
       , Func<T, TR> map) => @this.Map(map);

    public static Validation<TRR> SelectMany<T, TR, TRR>(this Validation<T> @this
       , Func<T, Validation<TR>> bind, Func<T, TR, TRR> project)
       => @this.Match(
          Invalid: (err) => Invalid(err),
          Valid: (t) => bind(t).Match(
             Invalid: (err) => Invalid(err),
             Valid: (r) => Valid(project(t, r))));
}
