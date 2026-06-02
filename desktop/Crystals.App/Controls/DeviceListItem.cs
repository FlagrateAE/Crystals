using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Crystals.Core.Devices;
using Crystals.Core.Utilities;
using Image = Wpf.Ui.Controls.Image;

namespace Crystals.App.Controls;

public sealed class DeviceListItem : Border
{
    private const int IconSize = 32;

    private readonly Image _statusIcon;

    public DeviceListItem(IDevice device)
    {
        BorderBrush = new SolidColorBrush(new Color { R = 63, G = 63, B = 63, A = 255 });
        BorderThickness = new Thickness(1);
        CornerRadius = new CornerRadius(8);
        Padding = new Thickness(8);
        Margin = new Thickness(20, 4, 20, 4);
        Background = Brushes.Transparent;

        MouseEnter += (_, _) => Background = new SolidColorBrush(new Color { R = 63, G = 63, B = 63, A = 255 });
        MouseLeave += (_, _) => Background = new SolidColorBrush(Colors.Transparent);

        var gridContent = new Grid();
        gridContent.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        gridContent.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        gridContent.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        Child = gridContent;

        var icon = new Image
        {
            Source = device.Icon,
            Width = IconSize,
            Height = IconSize,
            Stretch = Stretch.Uniform,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(5, 0, 0, 0)
        };
        Grid.SetColumn(icon, 0);
        gridContent.Children.Add(icon);

        var name = new TextBlock
        {
            Text = device.Name,
            FontSize = 18,
            TextAlignment = TextAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(name, 1);
        gridContent.Children.Add(name);

        _statusIcon = new Image
        {
            Width = IconSize * 0.8f,
            Height = IconSize * 0.8f,
            Stretch = Stretch.Uniform,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 0, 5, 0)
        };
        SetActive(false);
        Grid.SetColumn(_statusIcon, 2);
        gridContent.Children.Add(_statusIcon);
    }

    public void SetActive(bool active)
    {
        var iconUri = active ? ImageLoader.IconUris.Ok : ImageLoader.IconUris.Error;

        Application.Current.Dispatcher.Invoke(() => { _statusIcon.Source = BitmapFrame.Create(iconUri); });
    }
}