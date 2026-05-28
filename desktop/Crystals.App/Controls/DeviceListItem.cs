using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Crystals.Core.Devices;
using Image = Wpf.Ui.Controls.Image;

namespace Crystals.App.Controls;

public sealed class DeviceListItem : Border
{
    private const int IconSize = 32;

    public DeviceListItem(IDevice device)
    {
        BorderBrush = new SolidColorBrush(new Color { R = 63, G = 63, B = 63, A = 255 });
        BorderThickness = new Thickness(1);
        CornerRadius = new CornerRadius(8);
        Padding = new Thickness(8);
        Margin = new Thickness(0, 4, 0, 4);
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

        var stateLight = new Image
        {
            Source = BitmapFrame.Create(new Uri("pack://application:,,,/Resources/Ok.png")),
            Width = IconSize,
            Height = IconSize,
            Stretch = Stretch.Uniform,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 0, 5, 0)
        };
        Grid.SetColumn(stateLight, 2);
        gridContent.Children.Add(stateLight);
    }
}