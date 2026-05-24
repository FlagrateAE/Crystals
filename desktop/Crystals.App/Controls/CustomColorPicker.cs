using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Crystals.Core.Models;

namespace Crystals.App.Controls;

public class CustomColorPicker : Control
{
    private const double StartingHue = 0.0;

    public static readonly DependencyProperty HueProperty =
        DependencyProperty.Register(
            nameof(Hue),
            typeof(double),
            typeof(CustomColorPicker),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender, OnHueChanged));

    private double Hue
    {
        get => (double)GetValue(HueProperty);
        set => SetValue(HueProperty, Math.Clamp(value, 0.0, 360.0));
    }

    public CrystalsColor Color { get; }

    public event EventHandler<double>? HueChanged;

    private static void OnHueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is CustomColorPicker picker)
        {
            picker.HueChanged?.Invoke(picker, (double)e.NewValue);
        }
    }

    private const double ThicknessRatio = 0.5;
    private const double ThumbRadius = 8.0;
    private bool _isDragging;

    public CustomColorPicker()
    {
        Focusable = true;
        MinHeight = 100;
        MinWidth = 100;
        Hue = StartingHue;

        Color = CrystalsColor.FromHue(Hue);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        double size = Math.Min(RenderSize.Width, RenderSize.Height);
        if (size <= 0) return;

        Point center = new Point(RenderSize.Width / 2, RenderSize.Height / 2);
        double outerRadius = (size / 2) - ThumbRadius;
        double innerRadius = outerRadius * (1 - ThicknessRatio);

        int segments = 360;
        double angleStep = 360.0 / segments;

        for (int i = 0; i < segments; i++)
        {
            double startAngle = i * angleStep;
            double endAngle = (i + 1) * angleStep;

            var color = CrystalsColor.FromHue((startAngle + endAngle) / 2);
            Geometry arcSegment = CreateRingSegment(center, innerRadius, outerRadius, startAngle - 90, endAngle - 90);

            var brush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.RGB.R, color.RGB.G, color.RGB.B));
            brush.Freeze();
            var pen = new Pen(brush, 0.5);
            pen.Freeze();

            drawingContext.DrawGeometry(brush, pen, arcSegment);
        }

        double thumbAngleRad = (Hue - 90) * Math.PI / 180.0;
        double middleRadius = (innerRadius + outerRadius) / 2;
        Point thumbCenter = new Point(
            center.X + middleRadius * Math.Cos(thumbAngleRad),
            center.Y + middleRadius * Math.Sin(thumbAngleRad)
        );

        drawingContext.DrawEllipse(Brushes.Black, null, thumbCenter, ThumbRadius + 1, ThumbRadius + 1);
        drawingContext.DrawEllipse(Brushes.White, null, thumbCenter, ThumbRadius, ThumbRadius);

        var internalThumbBrush =
            new SolidColorBrush(System.Windows.Media.Color.FromRgb(Color.RGB.R, Color.RGB.G, Color.RGB.B));
        internalThumbBrush.Freeze();
        drawingContext.DrawEllipse(internalThumbBrush, null, thumbCenter, ThumbRadius - 3, ThumbRadius - 3);
    }

    private Geometry CreateRingSegment(Point center, double innerRadius, double outerRadius, double startAngleDeg,
        double endAngleDeg)
    {
        double startRad = startAngleDeg * Math.PI / 180.0;
        double endRad = endAngleDeg * Math.PI / 180.0;

        Point p1 = new Point(center.X + outerRadius * Math.Cos(startRad), center.Y + outerRadius * Math.Sin(startRad));
        Point p2 = new Point(center.X + outerRadius * Math.Cos(endRad), center.Y + outerRadius * Math.Sin(endRad));
        Point p3 = new Point(center.X + innerRadius * Math.Cos(endRad), center.Y + innerRadius * Math.Sin(endRad));
        Point p4 = new Point(center.X + innerRadius * Math.Cos(startRad), center.Y + innerRadius * Math.Sin(startRad));

        PathFigure figure = new PathFigure { StartPoint = p1, IsClosed = true };
        figure.Segments.Add(new ArcSegment(p2, new Size(outerRadius, outerRadius), 0, false, SweepDirection.Clockwise,
            true));
        figure.Segments.Add(new LineSegment(p3, true));
        figure.Segments.Add(new ArcSegment(p4, new Size(innerRadius, innerRadius), 0, false,
            SweepDirection.Counterclockwise, true));

        PathGeometry geometry = new PathGeometry();
        geometry.Figures.Add(figure);
        return geometry;
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);
        if (e.ChangedButton == MouseButton.Left)
        {
            CaptureMouse();
            _isDragging = true;
            UpdateHueFromMouse(e.GetPosition(this));
            e.Handled = true;
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (_isDragging)
        {
            UpdateHueFromMouse(e.GetPosition(this));
            e.Handled = true;
        }
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
        base.OnMouseUp(e);
        if (e.ChangedButton == MouseButton.Left && _isDragging)
        {
            _isDragging = false;
            ReleaseMouseCapture();
            e.Handled = true;
        }
    }

    private void UpdateHueFromMouse(Point mousePos)
    {
        var center = new Point(RenderSize.Width / 2, RenderSize.Height / 2);
        var delta = mousePos - center;

        var angleDeg = Math.Atan2(delta.Y, delta.X) * 180.0 / Math.PI;

        var hue = angleDeg + 90.0;
        if (hue < 0) hue += 360.0;

        Hue = hue;
    }
}