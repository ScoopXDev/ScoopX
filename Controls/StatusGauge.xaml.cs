using System;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI;

namespace ScoopX.Controls
{
    public sealed partial class StatusGauge : UserControl
    {
        private const double CautionThreshold = 80;
        private const double CriticalThreshold = 90;
        private const double AnimSpeed = 0.14; // 每帧逼近目标的比例，越小越柔
        private const double AnimSnapEpsilon = 0.08;

        private static readonly Color Accent = Color.FromArgb(255, 37, 99, 235);    // #2563EB 正常
        private static readonly Color Caution = Color.FromArgb(255, 249, 115, 22);  // #F97316 偏高警示
        private static readonly Color Critical = Color.FromArgb(255, 244, 63, 94);  // #F43F5E 超过90%
        private static readonly Color DetailDark = Color.FromArgb(255, 55, 65, 81);

        private double _displayPercent;
        private double _targetPercent;
        private bool _animating;
        private readonly DispatcherQueueTimer _animTimer;

        public StatusGauge()
        {
            InitializeComponent();
            _animTimer = DispatcherQueue.CreateTimer();
            _animTimer.Interval = TimeSpan.FromMilliseconds(16);
            _animTimer.IsRepeating = true;
            _animTimer.Tick += AnimTimer_Tick;
            Unloaded += (_, _) => _animTimer.Stop();
        }

        public void SetSimple(double percent, string detail, string label)
        {
            DetailText.Visibility = Visibility.Visible;
            UsagePanel.Visibility = Visibility.Collapsed;
            DetailText.Text = detail;
            DetailText.Foreground = new SolidColorBrush(DetailDark);
            LabelText.Text = label;
            AnimateTo(percent);
        }

        public void SetUsage(double percent, string used, string total, string label)
        {
            DetailText.Visibility = Visibility.Collapsed;
            UsagePanel.Visibility = Visibility.Visible;
            UsedText.Text = used;
            TotalText.Text = total;
            LabelText.Text = label;
            AnimateTo(percent);
        }

        private void AnimateTo(double percent)
        {
            _targetPercent = Math.Clamp(percent, 0, 100);
            if (!_animating && Math.Abs(_targetPercent - _displayPercent) < AnimSnapEpsilon)
            {
                _displayPercent = _targetPercent;
                ApplyVisual();
                return;
            }

            if (!_animating)
            {
                _animating = true;
                _animTimer.Start();
            }
        }

        private void AnimTimer_Tick(DispatcherQueueTimer sender, object args)
        {
            var delta = _targetPercent - _displayPercent;
            if (Math.Abs(delta) < AnimSnapEpsilon)
            {
                _displayPercent = _targetPercent;
                _animating = false;
                _animTimer.Stop();
                ApplyVisual();
                return;
            }

            // 指数缓动：越接近目标越慢，观感更自然
            _displayPercent += delta * AnimSpeed;
            ApplyVisual();
        }

        private void RingHost_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawArc(_displayPercent);
        }

        private void ApplyVisual()
        {
            var color = _displayPercent switch
            {
                > CriticalThreshold => Critical,
                >= CautionThreshold => Caution,
                _ => Accent,
            };
            var brush = new SolidColorBrush(color);
            ProgressPath.Stroke = brush;
            PercentText.Foreground = brush;
            UsedText.Foreground = brush;
            PercentText.Text = $"{_displayPercent:0}%";
            DrawArc(_displayPercent);
        }

        private void DrawArc(double percent)
        {
            var size = Math.Min(RingHost.ActualWidth, RingHost.ActualHeight);
            if (size <= 0)
            {
                return;
            }

            var stroke = ProgressPath.StrokeThickness;
            var radius = (size - stroke) / 2.0;
            var center = new Point(size / 2.0, size / 2.0);

            if (percent <= 0.05)
            {
                ProgressPath.Data = null;
                return;
            }

            var sweep = Math.Min(percent, 99.999) / 100.0 * 360.0;
            var start = PointOnCircle(center, radius, 0);
            var end = PointOnCircle(center, radius, sweep);

            var figure = new PathFigure
            {
                StartPoint = start,
                IsClosed = false,
            };
            figure.Segments.Add(new ArcSegment
            {
                Point = end,
                Size = new Size(radius, radius),
                IsLargeArc = sweep > 180,
                SweepDirection = SweepDirection.Clockwise,
                RotationAngle = 0,
            });

            var geometry = new PathGeometry();
            geometry.Figures.Add(figure);
            ProgressPath.Data = geometry;
        }

        private static Point PointOnCircle(Point center, double radius, double angleFromTopDegrees)
        {
            var radians = (angleFromTopDegrees - 90) * Math.PI / 180.0;
            return new Point(
                center.X + radius * Math.Cos(radians),
                center.Y + radius * Math.Sin(radians));
        }
    }
}
