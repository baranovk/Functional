using System.Diagnostics;
using static Functional.F;
using RUnit = System.ValueTuple;

namespace Functional.Tests.Unit;

internal sealed class MiddlewareTests
{
    private List<string> _sideEffects;
    private const int InitExceptionalValue = 10;
    private const int FinishExceptionalValue = 20;

    [SetUp]
    public void Setup()
    {
        _sideEffects = [];
    }

    [Test]
    public void Middleware_With_Exceptional_Should_RunInRightOrder()
    {
        Middleware<Exceptional<RUnit>> mw0 = (next) =>
        {
            _sideEffects.Add("Middleware0");

            return GetExceptional()
                .Match(
                    ex => ex,
                    u => next(Exceptional(u))
                );
        };

        Middleware<Exceptional<RUnit>> mw1 = (next) =>
        {
            _sideEffects.Add("Middleware1");

            var sw = new Stopwatch();
            _sideEffects.Add("Middleware1 sw.Start");
            sw.Start();

            var result = next(new RUnit());

            sw.Stop();
            _sideEffects.Add("Middleware1 sw.Stop");

            return result;
        };

        Middleware<Exceptional<RUnit>> mw2 = (next) =>
        {
            _sideEffects.Add("Middleware2");
            return next(new RUnit());
        };

        mw0
        .Bind(t => mw1)
        .Bind(t => mw2)
        .Run()
        .Match(
            ex => Assert.Fail(),
            u => Assert.Pass()
        );

        Assert.That(
            _sideEffects,
            Is.EqualTo(
                new List<string> { "Middleware0", "Middleware1", "Middleware1 sw.Start", "Middleware2", "Middleware1 sw.Stop" }
            )
        );
    }

    [Test]
    public async Task AsyncMiddleware_With_Exceptional_Should_RunInRightOrder()
    {
        AsyncMiddleware<Exceptional<int>> mw0 = async (next) =>
        {
            _sideEffects.Add("Middleware0");

            return await (await GetExceptionalAsync().ConfigureAwait(false))
                .Match(
                    ex => Async<dynamic>(Exceptional(ex)),
                    u => next(Exceptional(u))
                ).ConfigureAwait(false);
        };

        AsyncMiddleware<Exceptional<int>> mw1 = async (next) =>
        {
            _sideEffects.Add("Middleware1");

            var sw = new Stopwatch();
            _sideEffects.Add("Middleware1 sw.Start");
            sw.Start();

            await ExecuteAsync().ConfigureAwait(false);
            var result = await next(Exceptional(FinishExceptionalValue)).ConfigureAwait(false);

            sw.Stop();
            _sideEffects.Add("Middleware1 sw.Stop");

            return result;
        };

        (await mw0
            .Bind(t => mw1)
            .RunAsync()
            .ConfigureAwait(false)
        )
        .Match(
            ex => Assert.Fail(),
            u => Assert.That(u, Is.EqualTo(FinishExceptionalValue))
        );

        Assert.That(
            _sideEffects,
            Is.EqualTo(
                new List<string> { "Middleware0", "Middleware1", "Middleware1 sw.Start", "ExecuteAsync", "Middleware1 sw.Stop" }
            )
        );
    }

    private async Task ExecuteAsync()
    {
        _sideEffects.Add("ExecuteAsync");
        await Task.CompletedTask.ConfigureAwait(false);
    }

    private static async Task<Exceptional<int>> GetExceptionalAsync()
    {
        await Task.Yield();
        return Exceptional(InitExceptionalValue);
    }

    private static Exceptional<RUnit> GetExceptional() => Exceptional(new RUnit());
}
