using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Windows.Foundation;
using Windows.UI;
using IOPath = System.IO.Path;

namespace ScoopX.Pages
{
    public sealed partial class HomePage : Page
    {
        private const int HistoryCapacity = 48;

        private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromSeconds(1.2) };
        private readonly Queue<IoSample> _history = new();

        private ulong _prevIdle;
        private ulong _prevKernel;
        private ulong _prevUser;
        private bool _cpuPrimed;
        private double _chartMaxDisplay = 1024;

        private PerformanceCounter? _diskReadCounter;
        private PerformanceCounter? _diskWriteCounter;
        private PerformanceCounter? _diskOpsCounter;
        private PerformanceCounter? _diskLatencyCounter;
        private string _diskInstance = "_Total";

        private static readonly Color ReadColor = Color.FromArgb(255, 244, 114, 182);  // #F472B6
        private static readonly Color WriteColor = Color.FromArgb(255, 96, 165, 250);   // #60A5FA
        private static readonly Color GridColor = Color.FromArgb(255, 229, 231, 235);

        public HomePage()
        {
            InitializeComponent();
            _timer.Tick += (_, _) =>
            {
                RefreshMetrics();
                RefreshDiskIo();
            };
            InitDiskFilter();
            DiskFilterComboBox.SelectionChanged += DiskFilterComboBox_SelectionChanged;
            Unloaded += (_, _) => DisposeDiskCounters();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            EnsureDiskCounters();
            RefreshMetrics();
            RefreshDiskIo();
            _timer.Start();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _timer.Stop();
            base.OnNavigatedFrom(e);
        }

        private void InitDiskFilter()
        {
            DiskFilterComboBox.Items.Clear();
            DiskFilterComboBox.Items.Add(new ComboBoxItem { Content = "所有", Tag = "*" });

            foreach (var drive in DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.Fixed && d.IsReady))
            {
                var name = drive.Name.TrimEnd('\\', '/');
                DiskFilterComboBox.Items.Add(new ComboBoxItem { Content = name, Tag = name });
            }

            DiskFilterComboBox.SelectedIndex = 0;
        }

        private void DiskFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tag = (DiskFilterComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "*";
            _diskInstance = tag == "*" ? "_Total" : tag.TrimEnd('\\', '/');
            if (!_diskInstance.EndsWith(':') && _diskInstance != "_Total" && _diskInstance.Length == 1)
            {
                _diskInstance += ":";
            }

            DisposeDiskCounters();
            EnsureDiskCounters();
            _history.Clear();
            _chartMaxDisplay = 1024;
            RefreshDiskIo();
        }

        private void EnsureDiskCounters()
        {
            if (_diskReadCounter is not null)
            {
                return;
            }

            try
            {
                var instance = string.IsNullOrEmpty(_diskInstance) ? "_Total" : _diskInstance;
                _diskReadCounter = new PerformanceCounter("LogicalDisk", "Disk Read Bytes/sec", instance, readOnly: true);
                _diskWriteCounter = new PerformanceCounter("LogicalDisk", "Disk Write Bytes/sec", instance, readOnly: true);
                _diskOpsCounter = new PerformanceCounter("LogicalDisk", "Disk Transfers/sec", instance, readOnly: true);
                _diskLatencyCounter = new PerformanceCounter("LogicalDisk", "Avg. Disk sec/Transfer", instance, readOnly: true);

                // 第一次 NextValue 通常为 0，先预热建立基线
                _ = _diskReadCounter.NextValue();
                _ = _diskWriteCounter.NextValue();
                _ = _diskOpsCounter.NextValue();
                _ = _diskLatencyCounter.NextValue();
            }
            catch
            {
                DisposeDiskCounters();
            }
        }

        private void DisposeDiskCounters()
        {
            _diskReadCounter?.Dispose();
            _diskWriteCounter?.Dispose();
            _diskOpsCounter?.Dispose();
            _diskLatencyCounter?.Dispose();
            _diskReadCounter = null;
            _diskWriteCounter = null;
            _diskOpsCounter = null;
            _diskLatencyCounter = null;
        }

        private void RefreshDiskIo()
        {
            EnsureDiskCounters();

            double read = 0;
            double write = 0;
            double ops = 0;
            double latencyMs = 0;

            try
            {
                if (_diskReadCounter is not null
                    && _diskWriteCounter is not null
                    && _diskOpsCounter is not null
                    && _diskLatencyCounter is not null)
                {
                    read = Math.Max(0, _diskReadCounter.NextValue());
                    write = Math.Max(0, _diskWriteCounter.NextValue());
                    ops = Math.Max(0, _diskOpsCounter.NextValue());
                    latencyMs = Math.Max(0, _diskLatencyCounter.NextValue() * 1000.0);
                }
            }
            catch
            {
                DisposeDiskCounters();
            }

            var sample = new IoSample
            {
                Time = DateTime.Now,
                ReadBytesPerSec = read,
                WriteBytesPerSec = write,
                OpsPerSec = (int)Math.Round(ops),
                LatencyMs = latencyMs,
            };

            _history.Enqueue(sample);
            while (_history.Count > HistoryCapacity)
            {
                _history.Dequeue();
            }

            DiskReadText.Text = FormatRate(sample.ReadBytesPerSec);
            DiskWriteText.Text = FormatRate(sample.WriteBytesPerSec);
            DiskOpsText.Text = sample.OpsPerSec.ToString("0");
            DiskLatencyText.Text = $"{sample.LatencyMs:0} ms";
            DiskLatencyText.Foreground = new SolidColorBrush(
                sample.LatencyMs >= 20
                    ? Color.FromArgb(255, 249, 115, 22)
                    : Color.FromArgb(255, 34, 197, 94));

            DrawChart();
        }

        private void ChartCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawChart();
        }

        private void DrawChart()
        {
            if (ChartCanvas.ActualWidth < 10 || ChartCanvas.ActualHeight < 10 || _history.Count == 0)
            {
                return;
            }

            ChartCanvas.Children.Clear();
            ChartYLabels.Children.Clear();
            ChartXLabels.Children.Clear();

            var samples = _history.ToList();
            var rawMax = Math.Max(1, samples.Max(s => Math.Max(s.ReadBytesPerSec, s.WriteBytesPerSec)));
            var targetMax = Math.Max(1024, Math.Ceiling(rawMax / 5.0) * 5.0);
            // Y 轴刻度缓慢跟随，减少整图突然缩放
            _chartMaxDisplay += (targetMax - _chartMaxDisplay) * 0.22;
            var maxValue = Math.Max(1024, _chartMaxDisplay);

            var width = ChartCanvas.ActualWidth;
            var height = ChartCanvas.ActualHeight;
            var padTop = 8.0;
            var padBottom = 8.0;
            var plotHeight = height - padTop - padBottom;

            // 网格 + Y 轴
            for (var i = 0; i <= 5; i++)
            {
                var ratio = i / 5.0;
                var y = padTop + plotHeight * (1 - ratio);
                var value = maxValue * ratio;

                ChartCanvas.Children.Add(new Line
                {
                    X1 = 0,
                    Y1 = y,
                    X2 = width,
                    Y2 = y,
                    Stroke = new SolidColorBrush(GridColor),
                    StrokeThickness = 1,
                    StrokeDashArray = new DoubleCollection { 2, 3 },
                });

                var label = new TextBlock
                {
                    Text = FormatAxis(value),
                    FontSize = 10,
                    Foreground = new SolidColorBrush(Color.FromArgb(255, 156, 163, 175)),
                };
                Canvas.SetLeft(label, 0);
                Canvas.SetTop(label, Math.Max(0, y - 8));
                ChartYLabels.Children.Add(label);
            }

            DrawSeries(samples, s => s.WriteBytesPerSec, WriteColor, maxValue, width, padTop, plotHeight, fill: true);
            DrawSeries(samples, s => s.ReadBytesPerSec, ReadColor, maxValue, width, padTop, plotHeight, fill: false);

            // X 轴时间（首尾 + 中间若干）
            var labelCount = Math.Min(6, samples.Count);
            if (labelCount <= 0)
            {
                return;
            }

            for (var i = 0; i < labelCount; i++)
            {
                var index = labelCount == 1
                    ? 0
                    : (int)Math.Round(i * (samples.Count - 1) / (double)(labelCount - 1));
                var text = samples[index].Time.ToString("HH:mm:ss");
                ChartXLabels.Children.Add(new TextBlock
                {
                    Text = text,
                    FontSize = 10,
                    Foreground = new SolidColorBrush(Color.FromArgb(255, 156, 163, 175)),
                    Margin = new Thickness(0, 0, 12, 0),
                    Width = Math.Max(48, (width - 24) / labelCount),
                });
            }
        }

        private void DrawSeries(
            List<IoSample> samples,
            Func<IoSample, double> selector,
            Color color,
            double maxValue,
            double width,
            double padTop,
            double plotHeight,
            bool fill)
        {
            if (samples.Count < 2)
            {
                return;
            }

            var anchors = new List<Point>(samples.Count);
            for (var i = 0; i < samples.Count; i++)
            {
                var x = i * width / (samples.Count - 1);
                var ratio = Math.Clamp(selector(samples[i]) / maxValue, 0, 1);
                var y = padTop + plotHeight * (1 - ratio);
                anchors.Add(new Point(x, y));
            }

            // Catmull-Rom 采样成平滑折线
            var smooth = BuildSmoothPoints(anchors, segmentsPerSpan: 8);

            if (fill && smooth.Count > 1)
            {
                var fillPoints = new PointCollection { new Point(smooth[0].X, padTop + plotHeight) };
                foreach (var p in smooth)
                {
                    fillPoints.Add(p);
                }

                fillPoints.Add(new Point(smooth[^1].X, padTop + plotHeight));

                ChartCanvas.Children.Add(new Polygon
                {
                    Points = fillPoints,
                    Fill = new SolidColorBrush(Color.FromArgb(40, color.R, color.G, color.B)),
                    StrokeThickness = 0,
                });
            }

            var linePoints = new PointCollection();
            foreach (var p in smooth)
            {
                linePoints.Add(p);
            }

            ChartCanvas.Children.Add(new Polyline
            {
                Points = linePoints,
                Stroke = new SolidColorBrush(color),
                StrokeThickness = 2,
                StrokeLineJoin = PenLineJoin.Round,
                Fill = new SolidColorBrush(Colors.Transparent),
            });

            // 只在原始锚点打点，更干净
            for (var i = 0; i < anchors.Count; i++)
            {
                if (i % 4 != 0 && i != anchors.Count - 1)
                {
                    continue;
                }

                var p = anchors[i];
                var dot = new Ellipse
                {
                    Width = 4,
                    Height = 4,
                    Fill = new SolidColorBrush(color),
                };
                Canvas.SetLeft(dot, p.X - 2);
                Canvas.SetTop(dot, p.Y - 2);
                ChartCanvas.Children.Add(dot);
            }
        }

        private static List<Point> BuildSmoothPoints(IReadOnlyList<Point> anchors, int segmentsPerSpan)
        {
            var result = new List<Point>();
            if (anchors.Count == 0)
            {
                return result;
            }

            if (anchors.Count == 1)
            {
                result.Add(anchors[0]);
                return result;
            }

            for (var i = 0; i < anchors.Count - 1; i++)
            {
                var p0 = anchors[Math.Max(i - 1, 0)];
                var p1 = anchors[i];
                var p2 = anchors[i + 1];
                var p3 = anchors[Math.Min(i + 2, anchors.Count - 1)];

                for (var s = 0; s < segmentsPerSpan; s++)
                {
                    var t = s / (double)segmentsPerSpan;
                    result.Add(CatmullRom(p0, p1, p2, p3, t));
                }
            }

            result.Add(anchors[^1]);
            return result;
        }

        private static Point CatmullRom(Point p0, Point p1, Point p2, Point p3, double t)
        {
            var t2 = t * t;
            var t3 = t2 * t;
            var x = 0.5 * ((2 * p1.X) + (-p0.X + p2.X) * t
                + (2 * p0.X - 5 * p1.X + 4 * p2.X - p3.X) * t2
                + (-p0.X + 3 * p1.X - 3 * p2.X + p3.X) * t3);
            var y = 0.5 * ((2 * p1.Y) + (-p0.Y + p2.Y) * t
                + (2 * p0.Y - 5 * p1.Y + 4 * p2.Y - p3.Y) * t2
                + (-p0.Y + 3 * p1.Y - 3 * p2.Y + p3.Y) * t3);
            return new Point(x, y);
        }

        private static string FormatRate(double bytesPerSec)
        {
            if (bytesPerSec < 1024)
            {
                return $"{bytesPerSec:0} B";
            }

            if (bytesPerSec < 1024 * 1024)
            {
                return $"{bytesPerSec / 1024:0.0} KB";
            }

            return $"{bytesPerSec / (1024 * 1024):0.00} MB";
        }

        private static string FormatAxis(double bytesPerSec)
        {
            if (bytesPerSec < 1024)
            {
                return $"{bytesPerSec:0}";
            }

            if (bytesPerSec < 1024 * 1024)
            {
                return $"{bytesPerSec / 1024:0}";
            }

            return $"{bytesPerSec / (1024 * 1024):0.#}M";
        }

        private void RefreshMetrics()
        {
            var cpu = SampleCpuPercent();
            var cores = Environment.ProcessorCount;

            LoadGauge.SetSimple(cpu, DescribeLoad(cpu), "负载");
            CpuGauge.SetSimple(cpu, $"{cores}核心", "CPU");

            if (TryGetMemory(out var memUsed, out var memTotal, out var memPercent))
            {
                MemoryGauge.SetUsage(memPercent, FormatBytes(memUsed), FormatBytes(memTotal), "内存");
            }

            if (TryGetSystemDrive(out var diskUsed, out var diskTotal, out var diskPercent, out var label))
            {
                DiskGauge.SetUsage(diskPercent, FormatBytes(diskUsed), FormatBytes(diskTotal), label);
            }
        }

        private static string DescribeLoad(double percent) => percent switch
        {
            < 40 => "运行流畅",
            < 70 => "负载适中",
            < 90 => "负载偏高",
            _ => "负载过高",
        };

        private double SampleCpuPercent()
        {
            if (!GetSystemTimes(out var idleFt, out var kernelFt, out var userFt))
            {
                return 0;
            }

            var idle = ToUInt64(idleFt);
            var kernel = ToUInt64(kernelFt);
            var user = ToUInt64(userFt);

            if (!_cpuPrimed)
            {
                _prevIdle = idle;
                _prevKernel = kernel;
                _prevUser = user;
                _cpuPrimed = true;
                return 0;
            }

            var idleDelta = idle - _prevIdle;
            var kernelDelta = kernel - _prevKernel;
            var userDelta = user - _prevUser;
            _prevIdle = idle;
            _prevKernel = kernel;
            _prevUser = user;

            var total = kernelDelta + userDelta;
            if (total == 0)
            {
                return 0;
            }

            var busy = total - idleDelta;
            return Math.Clamp(busy * 100.0 / total, 0, 100);
        }

        private static bool TryGetMemory(out ulong used, out ulong total, out double percent)
        {
            used = 0;
            total = 0;
            percent = 0;

            var status = new MEMORYSTATUSEX { dwLength = (uint)Marshal.SizeOf<MEMORYSTATUSEX>() };
            if (!GlobalMemoryStatusEx(ref status))
            {
                return false;
            }

            total = status.ullTotalPhys;
            used = status.ullTotalPhys - status.ullAvailPhys;
            percent = total == 0 ? 0 : used * 100.0 / total;
            return true;
        }

        private static bool TryGetSystemDrive(out ulong used, out ulong total, out double percent, out string label)
        {
            used = 0;
            total = 0;
            percent = 0;
            label = "/";

            try
            {
                var root = IOPath.GetPathRoot(Environment.SystemDirectory) ?? @"C:\";
                var drive = new DriveInfo(root);
                if (!drive.IsReady)
                {
                    return false;
                }

                total = (ulong)drive.TotalSize;
                used = total - (ulong)drive.TotalFreeSpace;
                percent = total == 0 ? 0 : used * 100.0 / total;
                label = root.TrimEnd('\\', '/');
                if (string.IsNullOrEmpty(label))
                {
                    label = "/";
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string FormatBytes(ulong bytes)
        {
            const double gb = 1024d * 1024d * 1024d;
            const double mb = 1024d * 1024d;
            if (bytes >= gb)
            {
                return $"{bytes / gb:0.#}GB";
            }

            return $"{bytes / mb:0.#}MB";
        }

        private static ulong ToUInt64(FILETIME ft) => ((ulong)ft.dwHighDateTime << 32) | ft.dwLowDateTime;

        private sealed class IoSample
        {
            public DateTime Time { get; init; }
            public double ReadBytesPerSec { get; init; }
            public double WriteBytesPerSec { get; init; }
            public int OpsPerSec { get; init; }
            public double LatencyMs { get; init; }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct FILETIME
        {
            public uint dwLowDateTime;
            public uint dwHighDateTime;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetSystemTimes(out FILETIME idleTime, out FILETIME kernelTime, out FILETIME userTime);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);
    }
}
