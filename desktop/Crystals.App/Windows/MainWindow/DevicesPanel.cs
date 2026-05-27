using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Crystals.Core;
using Crystals.Core.Utilities;
using Color = System.Windows.Media.Color;
using Image = Wpf.Ui.Controls.Image;

namespace Crystals.App.Windows.MainWindow;

public sealed class DevicesPanel : Border
{
    const int MarginVertical = 40;
    const int MarginHorizontal = 80;

    private readonly CrystalsEngine _engine;

    private readonly StackPanel _content = new();
    private readonly Image _image;

    public DevicesPanel(CrystalsEngine engine)
    {
        _engine = engine;
        _engine.OnEngineColorChanged += _ => OnEngineStateChanged();
        _engine.OnSourceFocused += _ => OnEngineStateChanged();

        Background = new SolidColorBrush(new Color { R = 41, G = 41, B = 41, A = 150 });
        CornerRadius = new CornerRadius(16);
        BorderBrush = new SolidColorBrush(new Color { R = 63, G = 63, B = 63, A = 255 });
        BorderThickness = new Thickness(1);
        Padding = new Thickness(16);
        Margin = new Thickness(MarginHorizontal, MarginVertical, MarginHorizontal, MarginVertical);
        Child = _content;

        _image = new Image
        {
            Width = 300,
            Height = 300,
            Stretch = Stretch.Uniform,
            Margin = new Thickness(0, 0, 0, 16)
        };
        _content.Children.Add(_image);
    }

    private void OnEngineStateChanged()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            var currentSource = _engine.FocusedSource!.CurrentSource!;
            _image.Source = currentSource.Image.ToBitmapSource();
        });
    }
}