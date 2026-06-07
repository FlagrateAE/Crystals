using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Crystals.Core;
using Crystals.Core.Devices;
using Crystals.Core.Models;
using Crystals.Core.Sources;

namespace Crystals.App.Controls;

public sealed class DevicesPanel : Border, IDisposable
{
    private const int MarginVertical = 40;
    private const int MarginHorizontal = 80;
    private const int ContentWidth = 280;

    private readonly CrystalsEngine _engine;
    private readonly SolidColorBrush _borderBrush = new(new Color { R = 63, G = 63, B = 63, A = 255 });
    private readonly CrystalsColorPicker _colorPicker;
    private readonly Dictionary<IDevice, DeviceListItem> _deviceItems = new();

    public DevicesPanel(CrystalsEngine engine)
    {
        _engine = engine;
        _engine.OnDeviceSetActive += OnDeviceSetActive;
        _engine.OnStateChanged += OnEngineStateChanged;
        _engine.OnManualOverride += OnEngineManualOverride;

        var content = new Grid();

        Background = new SolidColorBrush(new Color { R = 41, G = 41, B = 41, A = 150 });
        CornerRadius = new CornerRadius(16);
        BorderBrush = _borderBrush;
        BorderThickness = new Thickness(1);
        Padding = new Thickness(16);
        Margin = new Thickness(MarginHorizontal, MarginVertical, MarginHorizontal, MarginVertical);
        Child = content;

        content.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        content.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        _colorPicker = new CrystalsColorPicker
        {
            Width = ContentWidth,
            Height = ContentWidth,
            Margin = new Thickness(20),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Top
        };
        _colorPicker.OnColorChanged += engine.ManualOverride;
        Grid.SetRow(_colorPicker, 0);
        content.Children.Add(_colorPicker);

        var devicesList = new StackPanel()
        {
            Orientation = Orientation.Vertical,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Background = Brushes.Transparent,
        };

        foreach (var device in engine.Devices)
        {
            var item = new DeviceListItem(device);
            devicesList.Children.Add(item);
            _deviceItems.Add(device, item);
        }

        Grid.SetRow(devicesList, 1);
        content.Children.Add(devicesList);
    }

    private void OnEngineManualOverride(CrystalsColor color)
    {
        SetColor(color);
    }

    private void OnDeviceSetActive(IDevice device, bool active)
    {
        _deviceItems[device].SetActive(active);
    }

    private void OnEngineStateChanged(ISource? _, CrystalsColor color)
    {
        Application.Current.Dispatcher.Invoke(() => { _colorPicker.Color = color; });
        SetColor(color);
    }

    private void SetColor(CrystalsColor color)
    {
        Application.Current.Dispatcher.Invoke(() => { _borderBrush.Color = color.ToRgb(); });
    }

    public void Dispose()
    {
        _colorPicker.OnColorChanged -= _engine.ManualOverride;
        _engine.OnStateChanged -= OnEngineStateChanged;
    }
}