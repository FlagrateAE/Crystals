using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Crystals.Core;
using Crystals.Core.Models;
using Crystals.Core.Sources;
using Crystals.Core.Utilities;
using ColorConverter = Crystals.Core.Utilities.ColorConverter;
using Image = Wpf.Ui.Controls.Image;

namespace Crystals.App.Controls;

public class EngineConnector : Grid, IDisposable
{
    private const int IconSize = 70;

    private readonly CrystalsEngine _engine;
    private readonly SolidColorBrush _borderBrush = new(new Color { R = 63, G = 63, B = 63, A = 255 });
    private readonly Canvas _linesCanvas;
    private Line _sourceLine;
    private Line _deviceLine;

    public EngineConnector(SourcesPanel sourcesPanel, CrystalsEngine engine, DevicesPanel devicesPanel)
    {
        _engine = engine;
        engine.OnStateChanged += OnEngineStateChanged;

        var icon = new Image
        {
            Source = BitmapFrame.Create(ImageLoader.IconUris.Crystals),
            Width = IconSize,
            Height = IconSize,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Stretch = Stretch.Uniform,
            CornerRadius = new CornerRadius(IconSize),
            BorderBrush = _borderBrush,
            BorderThickness = new Thickness(1),
        };
        RenderOptions.SetBitmapScalingMode(icon, BitmapScalingMode.HighQuality);

        _linesCanvas = new Canvas
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
        };

        Application.Current.MainWindow!.Loaded += (_, _) => DrawLines(sourcesPanel, icon, devicesPanel);
    }

    private void OnEngineStateChanged(ISource? _, CrystalsColor newColor)
    {
        var sourceColorRgb = ColorConverter.HSVtoRGB(_engine.FocusedSource!.CurrentColor);
        var deviceColorRgb = ColorConverter.HSVtoRGB(newColor);

        Application.Current.Dispatcher.Invoke(() =>
        {
            _borderBrush.Color = Color.FromArgb(255, deviceColorRgb.R, deviceColorRgb.G, deviceColorRgb.B);
        });
    }

    private void DrawLines(SourcesPanel sourcesPanel, Image icon, DevicesPanel devicesPanel)
    {
        var (sourceLineStartPoint, sourceLineEndPoint) = CalculateSourceLinePoints(sourcesPanel);
        _sourceLine = new Line
        {
            X1 = sourceLineStartPoint.X,
            Y1 = sourceLineStartPoint.Y,
            X2 = sourceLineEndPoint.X,
            Y2 = sourceLineEndPoint.Y,
            Stroke = _borderBrush,
            StrokeThickness = 2
        };
        _linesCanvas.Children.Add(_sourceLine);

        var (deviceLineStartPoint, deviceLineEndPoint) = CalculateDeviceLinePoints(devicesPanel);
        _deviceLine = new Line
        {
            X1 = deviceLineStartPoint.X,
            Y1 = deviceLineStartPoint.Y,
            X2 = deviceLineEndPoint.X,
            Y2 = deviceLineEndPoint.Y,
            Stroke = _borderBrush,
            StrokeThickness = 2
        };
        _linesCanvas.Children.Add(_deviceLine);

        Children.Add(icon);
        Children.Add(_linesCanvas);
    }

    private (Point, Point) CalculateSourceLinePoints(SourcesPanel sourcesPanel)
    {
        var startingPoint = new Point(
            ActualWidth / 4 + sourcesPanel.ActualWidth / 2,
            ActualHeight / 2
        );
        var endingPoint = new Point(
            ActualWidth / 2 - IconSize / 2,
            ActualHeight / 2
        );

        return (startingPoint, endingPoint);
    }

    private (Point, Point) CalculateDeviceLinePoints(DevicesPanel devicesPanel)
    {
        var startingPoint = new Point(
            ActualWidth * 0.75 - devicesPanel.ActualWidth / 2,
            ActualHeight / 2
        );
        var endingPoint = new Point(
            ActualWidth / 2 + IconSize / 2,
            ActualHeight / 2
        );

        return (startingPoint, endingPoint);
    }


    public void Dispose()
    {
        _engine.OnStateChanged -= OnEngineStateChanged;
    }
}