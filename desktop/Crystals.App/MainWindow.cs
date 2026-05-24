using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Crystals.App.Controls;

namespace Crystals.App;

public class MainWindow : Window
{
    public MainWindow()
    {
        Title = "Flagrate Crystals";
        Width = 400;
        Height = 450;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        Background = new SolidColorBrush(Color.FromRgb(30, 30, 30));

        var hueSelector = new CustomColorPicker
        {
            Width = 250,
            Height = 250,
            Margin = new Thickness(20),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        
        var statusLabel = new TextBlock
        {
            Text = $"Selected Color: {hueSelector.Color.RGB}",
            Foreground = Brushes.White,
            FontSize = 18,
            FontWeight = FontWeights.SemiBold,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(10)
        };

        hueSelector.HueChanged += (s, e) =>
        {
            statusLabel.Text = $"Selected Hue: {e:F0}°";
        };

        var rootStack = new StackPanel
        {
            VerticalAlignment = VerticalAlignment.Center
        };
        rootStack.Children.Add(hueSelector);
        rootStack.Children.Add(statusLabel);

        Content = rootStack;
    }
}