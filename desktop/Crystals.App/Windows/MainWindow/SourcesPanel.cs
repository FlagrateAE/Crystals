using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Crystals.App.Controls;
using Crystals.Core;

namespace Crystals.App.Windows.MainWindow;

public sealed class SourcesPanel : Border
{
    private const int MarginVertical = 40;
    private const int MarginHorizontal = 80;
    
    private readonly CrystalsEngine _engine;
    private readonly SolidColorBrush _borderBrush = new(new Color { R = 63, G = 63, B = 63, A = 255 });
    private readonly CrystalsColorPicker _colorPicker;

    public SourcesPanel(CrystalsEngine engine)
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
        
        _colorPicker = new CrystalsColorPicker
        {
            Width = 280,
            Height = 280,
            Margin = new Thickness(20),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Top
        };
        _colorPicker.OnColorChanged += engine.ManualOverride;
        Grid.SetRow(_colorPicker, 0);
        content.Children.Add(_colorPicker);
    }
}