using Crystals.Core.Devices;
using Crystals.Core.Middlewares;
using Crystals.Core.Middlewares.Preprocessors;
using Crystals.Core.Models;
using Crystals.Core.Sources;
using Microsoft.Extensions.Hosting;

namespace Crystals.Core;

public class CrystalsEngine(
    IEnumerable<ISource> sources,
    IEnumerable<IPreprocessor> preprocessors,
    IEnumerable<IPostprocessor> postprocessors,
    IEnumerable<IDevice> devices
) : BackgroundService
{
    public List<IDevice> Devices { get; } = devices.ToList();
    public ISource? FocusedSource { get; private set; }

    public event Action<IDevice, bool>? OnDeviceSetActive;

    public event Action<ISource?, CrystalsColor>? OnStateChanged;
    public event Action<CrystalsColor>? OnManualOverride;

    public void ManualOverride(CrystalsColor color)
    {
        SetColor(color);
        OnManualOverride?.Invoke(color);
    }

    public void ResetManualOverride()
    {
        var resetColor = FocusedSource!.CurrentColor;

        resetColor = preprocessors.Aggregate(resetColor, (current, preprocessor) => preprocessor.Process(current));

        SetColorSmooth(resetColor);
        OnStateChanged?.Invoke(null, resetColor);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        foreach (var source in sources)
        {
            source.Start();
            source.OnColorChanged += OnColorChanged;
        }

        foreach (var device in Devices.ToList())
        {
            var status = device.Start();
            if (!status)
            {
                Devices.Remove(device);
            }

            OnDeviceSetActive?.Invoke(device, status);
        }

        Console.WriteLine($"[ENGINE] Engine started: {sources.Count()} sources, {Devices.Count} devices\n");

        return Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private void OnColorChanged(object? sender, CrystalsColor color)
    {
        if (sender == null) return;

        var source = (ISource)sender;

        var focusedState = TryFocusOn(source);

        if (focusedState == FocusedState.FailedLowPriority) return;
        
        color = preprocessors.Aggregate(color, (current, preprocessor) => preprocessor.Process(current));

        SetColorSmooth(color);

        var sentSource = focusedState == FocusedState.New ? source : null;
        var sentColor = color;
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
        
        color = postprocessors.Aggregate(color, (current, postprocessor) => postprocessor.Process(current));
        
        foreach (var device in Devices)
        {
            device.SetColor(color);
        }

        Console.WriteLine("");
    }

    private void SetColorSmooth(CrystalsColor color)
    {
        Console.WriteLine($"[ENGINE] Setting color {color}");
        
        color = postprocessors.Aggregate(color, (current, postprocessor) => postprocessor.Process(current));
        
        foreach (var device in Devices)
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

        foreach (var device in Devices)
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