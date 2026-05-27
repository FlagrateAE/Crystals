using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Crystals.Core;

namespace Crystals.App.Windows.MainWindow;

public sealed class DevicesPanel : Border
{
    const int MarginVertical = 40;
    const int MarginHorizontal = 80;
    
    private readonly StackPanel _content = new();

    public DevicesPanel(CrystalsEngine engine)
    {
        Background = new SolidColorBrush(new Color { R = 41, G = 41, B = 41, A = 150 });
        CornerRadius = new CornerRadius(16);
        BorderBrush = new SolidColorBrush(new Color { R = 63, G = 63, B = 63, A = 255 });
        BorderThickness = new Thickness(1);
        Padding = new Thickness(16);
        Margin = new Thickness(MarginHorizontal, MarginVertical, MarginHorizontal, MarginVertical);
        Child = _content;
    }
}