using Crystals.Core.Devices;
using Crystals.Core.Models;
using Crystals.Core.Sources;

namespace Crystals.Core;

public class Engine
{
    private readonly List<ISource> _sources = [];
    private readonly List<IDevice> _devices = [];

    public void RegisterSource(ISource source)
    {
        _sources.Add(source);
        source.OnColorChanged += SetColorSmooth;
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

    private void SetColorSmooth(Color color)
    {
        foreach (var device in _devices)
        {
            device.SetColorSmooth(color);
        }
    }
}