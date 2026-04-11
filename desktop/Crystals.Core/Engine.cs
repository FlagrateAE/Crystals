using Crystals.Core.Devices;
using Crystals.Core.Models;
using Crystals.Core.Sources;

namespace Crystals.Core;

public class Engine
{
    private readonly List<ISource> _sources = [];
    private readonly List<IDevice> _devices = [];

    private ISource? _focusedSource;

    public void RegisterSource(ISource source)
    {
        _sources.Add(source);
        source.OnColorChanged += color => OnColorChanged(source, color);
    }

    public void RegisterDevice(IDevice device)
    {
        _devices.Add(device);
    }

    public void Start()
    {
        foreach (var source in _sources)
        {
            source.Start();
        }

        foreach (var device in _devices)
        {
            device.Start();
        }
    }

    private void OnColorChanged(ISource source, CrystalsColor color)
    {
        if (!TryFocusOn(source)) return;

        Console.WriteLine($"Color: {color}");
        SetColorSmooth(color);
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
        foreach (var device in _devices)
        {
            device.SetColorSmooth(color);
        }
    }
}