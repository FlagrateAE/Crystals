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
    private ISource? _focusedSource;


    public void ManualOverride(CrystalsColor color)
    {
        SetColorSmooth(color);
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

        if (!TryFocusOn(source)) return;

        var finalColor = color;
        foreach (var middleware in middlewares)
        {
            finalColor = middleware.Process(finalColor);
        }

        SetColorSmooth(finalColor);
    }

    private bool TryFocusOn(ISource source)
    {
        if (_focusedSource == null)
        {
            FocusOn(source);
            return true;
        }

        if (_focusedSource == source)
            return true;

        if (_focusedSource.FocusPriority > source.FocusPriority)
            return false;

        FocusOn(source);
        return true;

        void FocusOn(ISource s) => _focusedSource = s;
    }


    private void SetColorSmooth(CrystalsColor color)
    {
        Console.WriteLine($"[ENGINE] Setting color {color}");
        foreach (var device in devices)
        {
            device.SetColorSmooth(color);
        }

        Console.WriteLine("\n");
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
}