using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Crystals.Core;
using Crystals.Core.Models;
using Crystals.Core.Sources;
using Crystals.Core.Utilities;
using Color = System.Windows.Media.Color;
using ColorConverter = Crystals.Core.Utilities.ColorConverter;
using Image = Wpf.Ui.Controls.Image;
using TextBlock = Wpf.Ui.Controls.TextBlock;

namespace Crystals.App.Controls;

public sealed class SourcesPanel : Border, IDisposable
{
    private const int MarginVertical = 40;
    private const int MarginHorizontal = 80;

    private const int ImageSize = 280;

    private readonly CrystalsEngine _engine;
    private readonly SolidColorBrush _borderBrush = new(new Color { R = 63, G = 63, B = 63, A = 255 });
    private readonly Image _sourceIcon;
    private readonly Image _image;
    private readonly TextBlock _title;
    private readonly TextBlock _description;

    public SourcesPanel(CrystalsEngine engine)
    {
        _engine = engine;
        _engine.OnStateChanged += OnEngineStateChanged;

        var contentIconWrapper = new Grid();
        var content = new Grid();
        contentIconWrapper.Children.Add(content);

        Background = new SolidColorBrush(new Color { R = 41, G = 41, B = 41, A = 150 });
        CornerRadius = new CornerRadius(16);
        BorderBrush = _borderBrush;
        BorderThickness = new Thickness(1);
        Padding = new Thickness(16);
        Margin = new Thickness(MarginHorizontal, MarginVertical, MarginHorizontal, MarginVertical);
        Child = contentIconWrapper;

        _sourceIcon = new Image
        {
            Width = 30,
            Height = 30,
            Stretch = Stretch.Uniform,
            CornerRadius = new CornerRadius(50),
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Bottom,
        };
        contentIconWrapper.Children.Add(_sourceIcon);

        // _content.ShowGridLines = true;
        content.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        content.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        content.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        _image = new Image
        {
            Width = ImageSize,
            Height = ImageSize,
            Stretch = Stretch.Uniform,
            Margin = new Thickness(25, 25, 25, 20),
            Source = ImageLoader.LoadFromUri(ImageLoader.IconUris.Crystals, ImageSize),
            CornerRadius = new CornerRadius(8),
            BorderBrush = new SolidColorBrush(new Color { R = 63, G = 63, B = 63, A = 255 }),
            BorderThickness = new Thickness(1),
        };
        Grid.SetRow(_image, 0);
        content.Children.Add(_image);

        _title = new TextBlock
        {
            Text = "None",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            FontWeight = FontWeights.Normal,
            FontSize = 24,
        };
        Grid.SetRow(_title, 1);
        content.Children.Add(_title);

        _description = new TextBlock
        {
            Text = "No source in focus",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = 16,
            Margin = new Thickness(0, 0, 0, 18),
        };
        Grid.SetRow(_description, 2);
        content.Children.Add(_description);
    }

    private void OnEngineStateChanged(ISource? newSource, CrystalsColor __)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            if (newSource != null)
            {
                _sourceIcon.Source = newSource.Icon;
            }

            var focusedSource = _engine.FocusedSource!;
            var currentSourceModel = focusedSource.CurrentSource!;
            var colorRgb = ColorConverter.HSVtoRGB(focusedSource.CurrentColor);
            _borderBrush.Color = Color.FromArgb(255, colorRgb.R, colorRgb.G, colorRgb.B);
            _image.Source = currentSourceModel.Image.ToBitmapSource();
            _title.Text = currentSourceModel.Name;
            _description.Text = currentSourceModel.Description;
        });
    }

    public void Dispose()
    {
        _engine.OnStateChanged -= OnEngineStateChanged;
    }
}