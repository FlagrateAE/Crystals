using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Crystals.Core;
using Crystals.Core.Models;
using Crystals.Core.Sources;
using ColorConverter = Crystals.Core.Utilities.ColorConverter;

namespace Crystals.App.Controls;

public sealed class DevicesPanel : Border, IDisposable
{
    private const int MarginVertical = 40;
    private const int MarginHorizontal = 80;
    private const int ContentWidth = 280;

    private readonly CrystalsEngine _engine;
    private readonly SolidColorBrush _borderBrush = new(new Color { R = 63, G = 63, B = 63, A = 255 });
    private readonly CrystalsColorPicker _colorPicker;

    public DevicesPanel(CrystalsEngine engine)
    {
        _engine = engine;

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
        _colorPicker.OnColorChanged += SetColor;
        _engine.OnStateChanged += OnEngineStateChanged;
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
        }

        Grid.SetRow(devicesList, 1);
        content.Children.Add(devicesList);
    }

    private void OnEngineStateChanged(ISource? _, CrystalsColor color)
    {
        Application.Current.Dispatcher.Invoke(() => { _colorPicker.Color = color; });
        SetColor(color);
    }

    private void SetColor(CrystalsColor color)
    {
        var rgbColor = ColorConverter.HSVtoRGB(color);
        Application.Current.Dispatcher.Invoke(() =>
        {
            _borderBrush.Color = Color.FromArgb(255, rgbColor.R, rgbColor.G, rgbColor.B);
        });
    }

    public void Dispose()
    {
        _colorPicker.OnColorChanged -= _engine.ManualOverride;
        _engine.OnStateChanged -= OnEngineStateChanged;
    }
}