using System;
using System.Windows;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Hardcodet.Wpf.TaskbarNotification;
using HolidayDesktop.ViewModels;
using HolidayDesktop.Views;
using HolidayDesktop.Settings;
using HolidayDesktop.Common;
using static HolidayDesktop.Common.NativeMethods;

namespace HolidayDesktop
{
    class ViewManager
    {
        private TaskbarIcon _tray;
        private TaskbarIconViewModel _trayViewModel;
        private List<MainWindow> _windows;
        private DateTime _toDay;
        private DispatcherTimer _timer;

        public ProgramSettings Settings { get; private set; } = new ProgramSettings();

        public ViewManager()
        {
            _windows = new List<MainWindow>();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(5);
            _timer.Tick += TimerTick;
        }

        public void ApplySettings(SettingsViewModel settings)
        {
            Settings.ViewType = settings.ThemeChecked ? ViewType.Theme : ViewType.SimplePath;
            Settings.ImagesDirectoryName = settings.ImagesDirectoryName;
            Settings.ImageFileName = settings.ImageFileName;
            Settings.ShowCenterImage = settings.ShowCenterImage;
            Settings.RunImageAnimation = settings.RunImageAnimation;
            Settings.IntervalBetweenImages = settings.IntervalBetweenImages;
            Settings.IntervalForShowImage = settings.IntervalForShowImage;
            Settings.Monitor = settings.Monitor;
            Settings.ChangeThemeWhenDayIsChanged = settings.ChangeThemeWhenDayIsChanged;
            Settings.Themes.Clear();
            Settings.Themes.AddRange(settings.Themes.Select(x => (ThemeSettings)x.Clone()).ToList());

            Stop();
            CreateWindows();
            Start();
            SaveSettings();
        }

        public void CreateWindows()
        {
            var windows = 0;
            foreach (var window in _windows)
            {
                window.AllowClose = true;
                window.Hide();
            }

            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, (IntPtr hMonitor, IntPtr hdcMonitor, ref Common.Rect rect, IntPtr data) =>
            {
                if (Settings.Monitor == null || Settings.Monitor == windows)
                {
                    var info = new MonitorInfo();
                    info.Init();
                    GetMonitorInfo(hMonitor, ref info);

                    var mainWindow = new MainWindow
                    {
                        Left = info.rcWork.Left,
                        Top = info.rcWork.Top,
                        Width = info.rcWork.Width,
                        Height = info.rcWork.Height,
                        DataContext = new MainWindowViewModel(Settings)
                    };

                    mainWindow.Show();
                    _windows.Add(mainWindow);
                }

                windows++;
                return true;
            }, IntPtr.Zero);

            foreach (var window in _windows)
            {
                if (window.AllowClose)
                {
                    window.Close();
                }
            }
            _windows.RemoveAll(x => x.AllowClose == true);
        }

        public void Start()
        {
            if (Settings.ViewType == ViewType.Theme && Settings.ChangeThemeWhenDayIsChanged)
            {
                _toDay = DateTime.Today;
                _timer.Start();
            }
            foreach (var window in _windows)
            {
                window.StartAnimation();
                window.Visibility = Visibility.Visible;
            }
        }

        public void Stop()
        {
            _timer.Stop();
            foreach (var window in _windows)
            {
                window.StopAnimation();
                window.Visibility = Visibility.Collapsed;
            }
        }

        public bool IsWindowVisible
        {
            get
            {
                return _windows.Any() && _windows[0].Visibility == Visibility.Visible;
            }
        }

        public void EnableTray(bool enable)
        {
            _tray.Visibility = enable ? Visibility.Visible : Visibility.Collapsed;
        }

        public void InitTray()
        {
            _tray = Application.Current.FindResource("TrayIcon") as TaskbarIcon;
            _trayViewModel = new TaskbarIconViewModel(this, Settings, DialogService.Instance);
            _tray.DataContext = _trayViewModel;
            TaskbarIcon.SetParentTaskbarIcon(Application.Current.MainWindow, _tray);
        }

        public void LoadSettings()
        {
            var settingsFileName = Path.GetFileNameWithoutExtension(AssemblyUtils.AssemblyLocation) + ".xml";
            settingsFileName = Path.Combine(AssemblyUtils.AssemblyDirectoryName, settingsFileName);
            if (File.Exists(settingsFileName))
            {
                try
                {
                    var xml = File.ReadAllText(settingsFileName, Encoding.UTF8);
                    Settings = SerializeUtils.Deserialize<ProgramSettings>(xml);
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Failed to load the file {settingsFileName}{Environment.NewLine}{e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveSettings()
        {
            var settingsFileName = Path.GetFileNameWithoutExtension(AssemblyUtils.AssemblyLocation) + ".xml";
            settingsFileName = Path.Combine(AssemblyUtils.AssemblyDirectoryName, settingsFileName);
            try
            {
                File.WriteAllText(settingsFileName, SerializeUtils.Serialize(Settings), Encoding.UTF8);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Failed to write to the file {settingsFileName}{Environment.NewLine}{e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            var toDay = DateTime.Today;
            if (_toDay != toDay)
            {
                _toDay = toDay;
                var toDayTheme = Settings.Themes.FirstOrDefault(x => x.Date.HasValue && x.Date.Value.Month == toDay.Month && x.Date.Value.Day == toDay.Day);
                if (toDayTheme != null)
                {
                    Settings.Themes.ForEach(x => x.IsActive = false);
                    toDayTheme.IsActive = true;
                    Stop();
                    CreateWindows();
                    Start();
                }
            }
        }
    }
}
