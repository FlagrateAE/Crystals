using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Crystals.App.Controls;
using Crystals.Core;
using Crystals.Core.Models;
using Crystals.Core.Sources;
using ListView = Wpf.Ui.Controls.ListView;
using TextBlock = Wpf.Ui.Controls.TextBlock;

namespace Crystals.App.Windows.MainWindow;

public sealed class DevicesPanel : Border, IDisposable
{
    private const int MarginVertical = 40;
    private const int MarginHorizontal = 80;
    private const int ContentWidth = 280;

    private readonly CrystalsEngine _engine;
    private readonly SolidColorBrush _borderBrush = new(new Color { R = 63, G = 63, B = 63, A = 255 });
    private readonly CrystalsColorPicker _colorPicker;
    private readonly ListView _devicesList;

    public DevicesPanel(CrystalsEngine engine)
    {
        _engine = engine;

        var content = new Grid();
        content.ShowGridLines = true;

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
        _engine.OnStateChanged += OnEngineStateChanged;
        Grid.SetRow(_colorPicker, 0);
        content.Children.Add(_colorPicker);

        _devicesList = new ListView
        {
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            Focusable = false,
        };
        var panelFactory = new FrameworkElementFactory(typeof(StackPanel));
        panelFactory.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
        panelFactory.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
        _devicesList.ItemsPanel = new ItemsPanelTemplate(panelFactory);
        foreach (var device in engine.Devices)
        {
            var item = new DeviceListItem(device);
            _devicesList.Items.Add(item);
        }

        Grid.SetRow(_devicesList, 1);
        content.Children.Add(_devicesList);
    }

    private void OnEngineStateChanged(ISource? _, CrystalsColor color)
    {
        Application.Current.Dispatcher.Invoke(() => { _colorPicker.Color = color; });
    }

    public void Dispose()
    {
        _colorPicker.OnColorChanged -= _engine.ManualOverride;
        _engine.OnStateChanged -= OnEngineStateChanged;
    }
}