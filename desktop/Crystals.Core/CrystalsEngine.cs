using Crystals.Core.Devices;
using Crystals.Core.Middlewares;
using Crystals.Core.Models;
using Crystals.Core.Sources;
using Microsoft.Extensions.Hosting;

namespace Crystals.Core;

public class CrystalsEngine(
    IEnumerable<ISource> sources,
    IEnumerable<IMiddleware> middlewares,
    IEnumerable<IDevice> devices
) : BackgroundService
{
    public IEnumerable<IDevice> Devices { get; } = devices;
    public ISource? FocusedSource { get; private set; }

    public event Action<ISource?, CrystalsColor>? OnStateChanged;

    public void ManualOverride(CrystalsColor color)
    {
        SetColor(color);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        foreach (var source in sources)
        {
            source.Start();
            source.OnColorChanged += OnColorChanged;
        }

        foreach (var device in devices)
        {
            device.Start();
        }

        Console.WriteLine("[ENGINE] Engine started\n");

        return Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private void OnColorChanged(object? sender, CrystalsColor color)
    {
        if (sender == null) return;

        var source = (ISource)sender;

        var focusedState = TryFocusOn(source);

        if (focusedState == FocusedState.FailedLowPriority) return;

        var finalColor = color;
        foreach (var middleware in middlewares)
        {
            finalColor = middleware.Process(finalColor);
        }

        SetColorSmooth(finalColor);

        var sentSource = focusedState == FocusedState.New ? source : null;
        var sentColor = finalColor;
        OnStateChanged?.Invoke(sentSource, sentColor);
    }

    private FocusedState TryFocusOn(ISource source)
    {
        if (FocusedSource == null)
        {
            FocusOn(source);
            return FocusedState.New;
        }

        if (FocusedSource == source)
            return FocusedState.Same;

        if (FocusedSource.FocusPriority > source.FocusPriority)
            return FocusedState.FailedLowPriority;

        FocusOn(source);
        return FocusedState.New;

        void FocusOn(ISource s) => FocusedSource = s;
    }

    private void SetColor(CrystalsColor color)
    {
        Console.WriteLine($"[ENGINE] Setting color {color}");
        foreach (var device in devices)
        {
            device.SetColor(color);
        }

        Console.WriteLine("");
    }


    private void SetColorSmooth(CrystalsColor color)
    {
        Console.WriteLine($"[ENGINE] Setting color {color}");
        foreach (var device in devices)
        {
            device.SetColorSmooth(color);
        }

        Console.WriteLine("");
    }

    public override void Dispose()
    {
        foreach (var source in sources)
        {
            source.OnColorChanged -= OnColorChanged;
        }

        foreach (var device in devices)
        {
            device.Stop();
        }

        base.Dispose();
    }

    private enum FocusedState
    {
        New,
        Same,
        FailedLowPriority
    }
}