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

    public static async Task<T> IterateUntilAsync<T>(
        this Task<T> @this,
        Func<T, Task<T>> update,
        Func<T, bool> endCondition)
    {
        var currentThis = await @this.ConfigureAwait(false);

        while (!endCondition(currentThis))
        {
            currentThis = await update(currentThis).ConfigureAwait(false);
        }

        return currentThis;
    }
}
