namespace Functional;

public static class Extensions
{
    public static T IterateUntil<T>(
        this T @this,
        Func<T, T> update,
        Func<T, bool> endCondition)
    {
        var currentThis = @this;

        while (!endCondition(currentThis))
        {
            currentThis = update(currentThis);
        }

        return currentThis;
    }
}
