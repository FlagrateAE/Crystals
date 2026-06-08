using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Crystals.Core;
using Crystals.Core.Models;
using Crystals.Core.Sources;
using Crystals.Core.Utilities;
using Image = Wpf.Ui.Controls.Image;

namespace Crystals.App.Controls;

public class EngineConnector : Grid, IDisposable
{
    private const int IconSize = 70;

    private readonly CrystalsEngine _engine;
    private readonly SolidColorBrush _sourceLineBrush = new(new Color { R = 63, G = 63, B = 63, A = 255 });
    private readonly SolidColorBrush _borderBrush = new(new Color { R = 63, G = 63, B = 63, A = 255 });
    private readonly SolidColorBrush _deviceLineBrush = new(new Color { R = 63, G = 63, B = 63, A = 255 });
    private readonly Canvas _linesCanvas;
    private readonly Image _icon;
    private readonly BitmapSource _defaultIconSource;
    private readonly BitmapSource _manualOverrideSaveIconSource;
    private Line _sourceLine;
    private Line _deviceLine;

    public EngineConnector(SourcesPanel sourcesPanel, CrystalsEngine engine, DevicesPanel devicesPanel)
    {
        _engine = engine;
        engine.OnStateChanged += OnEngineStateChanged;
        engine.OnManualOverride += OnEngineManualOverride;

        _defaultIconSource = BitmapFrame.Create(ImageLoader.IconUris.Crystals);
        _manualOverrideSaveIconSource = BitmapFrame.Create(new Uri("pack://application:,,,/Resources/Save.png"));

        _icon = new Image
        {
            Source = _defaultIconSource,
            Width = IconSize,
            Height = IconSize,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Stretch = Stretch.Uniform,
            CornerRadius = new CornerRadius(IconSize),
            BorderBrush = _borderBrush,
            BorderThickness = new Thickness(1),
        };
        _icon.MouseUp += OnIconClick;
        RenderOptions.SetBitmapScalingMode(_icon, BitmapScalingMode.HighQuality);

        _linesCanvas = new Canvas
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
        };

        Application.Current.MainWindow!.Loaded += (_, _) => DrawLines(sourcesPanel, _icon, devicesPanel);
    }

    private void OnEngineStateChanged(ISource? _, CrystalsColor newColor)
    {
        var sourceColor = _engine.FocusedSource!.CurrentColor.ToRgb();
        var deviceColor = newColor.ToRgb();
        Application.Current.Dispatcher.Invoke(() =>
        {
            _icon.Source = _defaultIconSource;
            _sourceLineBrush.Color = sourceColor;
            _borderBrush.Color = deviceColor;
            _deviceLineBrush.Color = deviceColor;
        });
    }

    private void OnEngineManualOverride(CrystalsColor color)
    {
        if (_engine.FocusedSource == null) return;
        _icon.Source = _manualOverrideSaveIconSource;
        _deviceLineBrush.Color = color.ToRgb();
    }

    private void OnIconClick(object sender, MouseButtonEventArgs e)
    {
        if (_icon.Source == _defaultIconSource) return;

        _engine.SaveManualOverride();
        _icon.Source = _defaultIconSource;
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
            Stroke = _sourceLineBrush,
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
            Stroke = _deviceLineBrush,
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