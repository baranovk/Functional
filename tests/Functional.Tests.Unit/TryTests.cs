using static Functional.F;

namespace Functional.Tests.Unit;

internal sealed class TryTests
{
    [Test]
    public async Task TryAsync_Should_RunAsync_Without_Exception()
    {
        var testUri = new Uri("http://localhost");
        var createUri = async () => { await Task.Yield(); return testUri; };

        (await TryAsync(createUri).RunAsync().ConfigureAwait(false))
            .Match(_ => Assert.Fail(), uri => Assert.That(uri, Is.EqualTo(testUri)));
    }

    [Test]
    public async Task TryAsync_Should_RunAsync_With_Exception()
    {
        var createUri = async () => { await Task.Yield(); return new Uri(""); };

        (await TryAsync(createUri).RunAsync().ConfigureAwait(false))
            .Match(_ => Assert.Pass(), _ => Assert.Fail());
    }

    [Test]
    public async Task TryAsync_Should_MapAsync_Without_Exception()
    {
        const int divisible = 35;
        const int divider = 5;

        var divide = async () => { await Task.Yield(); return divisible / divider; };

        (await TryAsync(divide)
                .MapAsync(
                    ex => ex.Match(
                        _ => throw _,
                        t => Task.FromResult(Math.Pow(t, 2))
                    ))
                    .RunAsync()
                    .ConfigureAwait(false)
        ).Match(_ => Assert.Fail(), val => Assert.That(val, Is.EqualTo(49)));
    }

    [Test]
    public async Task TryAsync_Should_MapAsync_With_Exception()
    {
        const int divisible = 35;
        var divider = 0;

        var divide = async () => { await Task.Yield(); return divisible / divider; };

        (await TryAsync(divide)
                .MapAsync(
                    ex => ex.Match(
                        _ => throw _,
                        t => Task.FromResult(Math.Pow(t, 2))
                    ))
                    .RunAsync()
                    .ConfigureAwait(false)
        ).Match(_ => Assert.Pass(), _ => Assert.Fail());
    }

    [Test]
    public async Task TryAsync_Should_Bind_Without_Exception()
    {
        const int divisible = 35;
        const int divider = 5;

        var divide = async () => { await Task.Yield(); return divisible / divider; };

        (await TryAsync(divide)
                .Bind(
                    ex => ex.Match(
                        _ => throw _,
                        t => TryAsync(() => Task.FromResult(Math.Pow(t, 2)))
                    ))
                    .RunAsync()
                    .ConfigureAwait(false)
        ).Match(_ => Assert.Fail(), val => Assert.That(val, Is.EqualTo(49)));
    }

    [Test]
    public async Task TryAsync_Should_Bind_With_Exception()
    {
        const int divisible = 35;
        var divider = 0;

        var divide = async () => { await Task.Yield(); return divisible / divider; };

        (await TryAsync(divide)
                .Bind(
                    ex => ex.Match(
                        _ => throw _,
                        t => TryAsync(() => Task.FromResult(Math.Pow(t, 2)))
                    ))
                    .RunAsync()
                    .ConfigureAwait(false)
        ).Match(_ => Assert.Pass(), _ => Assert.Fail());
    }
}
