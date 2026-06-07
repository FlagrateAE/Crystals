using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Crystals.Core.Models;

namespace Crystals.App.Controls;

public class CrystalsColorPicker : Control
{
    private const float StartingHue = 0.0f;

    private double _hue;

    public CrystalsColor Color
    {
        get;
        set
        {
            field = value;
            _hue = value.H;
            InvalidateVisual();
        }
    }

    public event Action<CrystalsColor>? OnColorChanged;

    private const double ThicknessRatio = 0.5;
    private const double InnerWhiteRadius = 30;
    private const double ThumbRadius = 8.0;
    private const int ThrottleIntervalMs = 50;

    private readonly Stopwatch _throttleStopwatch = new();
    private bool _isDragging;
    private Point _center;

    public CrystalsColorPicker()
    {
        Focusable = true;
        MinHeight = 100;
        MinWidth = 100;
        _hue = StartingHue;

        Color = new CrystalsColor((float)_hue, 1, 1);
        _throttleStopwatch.Start();
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        double size = Math.Min(RenderSize.Width, RenderSize.Height);
        if (size <= 0) return;

        _center = new Point(RenderSize.Width / 2, RenderSize.Height / 2);
        double outerRadius = (size / 2) - ThumbRadius;
        double innerRadius = outerRadius * (1 - ThicknessRatio);

        int segments = 360;
        float angleStep = 360.0f / segments;

        var drawingCrystalsColor = new CrystalsColor(0f, 1f, 1f);

        for (int i = 0; i < segments; i++)
        {
            float startAngle = i * angleStep;
            float endAngle = (i + 1) * angleStep;

            drawingCrystalsColor = drawingCrystalsColor.WithH((startAngle + endAngle) / 2);
            var color = drawingCrystalsColor.ToRgb();
            Geometry arcSegment = CreateRingSegment(_center, innerRadius, outerRadius, startAngle - 90, endAngle - 90);

            var brush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
            brush.Freeze();
            var pen = new Pen(brush, 0.5);
            pen.Freeze();

            drawingContext.DrawGeometry(brush, pen, arcSegment);
        }

        drawingContext.DrawEllipse(Brushes.White, null, _center, InnerWhiteRadius, InnerWhiteRadius);

        double thumbAngleRad = (_hue - 90) * Math.PI / 180.0;
        double middleRadius = (innerRadius + outerRadius) / 2;
        Point thumbCenter = new Point(
            _center.X + middleRadius * Math.Cos(thumbAngleRad),
            _center.Y + middleRadius * Math.Sin(thumbAngleRad)
        );

        drawingContext.DrawEllipse(Brushes.Black, null, thumbCenter, ThumbRadius + 1, ThumbRadius + 1);
        drawingContext.DrawEllipse(Brushes.White, null, thumbCenter, ThumbRadius, ThumbRadius);

        var internalThumbBrush =
            new SolidColorBrush(Color.ToRgb());
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
            UpdateColorFromMouse(e.GetPosition(this), forceUpdate: true);

            e.Handled = true;
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (_isDragging)
        {
            UpdateColorFromMouse(e.GetPosition(this), forceUpdate: false);
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
            UpdateColorFromMouse(e.GetPosition(this), forceUpdate: true);
            e.Handled = true;
        }
    }

    private void UpdateColorFromMouse(Point mousePos, bool forceUpdate)
    {
        var center = new Point(RenderSize.Width / 2, RenderSize.Height / 2);

        var isWhiteCircle = (mousePos - center).Length < InnerWhiteRadius;

        if (isWhiteCircle)
        {
            Color = CrystalsColor.White;
        }
        else
        {
            var delta = mousePos - center;

            var angleDeg = Math.Atan2(delta.Y, delta.X) * 180.0 / Math.PI;

            var hue = angleDeg + 90.0;
            if (hue < 0) hue += 360.0;

            _hue = hue;
            Color = new CrystalsColor((float)hue, 1, 1);
        }
        
        InvalidateVisual();

        if (forceUpdate || _throttleStopwatch.ElapsedMilliseconds >= ThrottleIntervalMs)
        {
            OnColorChanged?.Invoke(Color);
            _throttleStopwatch.Restart();
        }
    }
}