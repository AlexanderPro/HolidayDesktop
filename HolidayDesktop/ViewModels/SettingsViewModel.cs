using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using HolidayDesktop.Common;
using HolidayDesktop.Settings;
using static HolidayDesktop.Common.NativeMethods;

namespace HolidayDesktop.ViewModels
{
    sealed class SettingsViewModel : DialogViewModelBase
    {
        public ProgramSettings Settings { get; }

        public SettingsViewModel(Window dialog, ProgramSettings settings) : base(dialog)
        {
            Settings = settings;

            _themeChecked = settings.ViewType == ViewType.Theme;
            _pathChecked = settings.ViewType == ViewType.SimplePath;
            _imagesDirectoryName = PathUtils.MakeRelativePath(settings.ImagesAbsoluteDirectoryName, AssemblyUtils.AssemblyDirectoryName);
            _imageFileName = PathUtils.MakeRelativePath(settings.ImageAbsoluteFileName, AssemblyUtils.AssemblyDirectoryName);
            _showCenterImage = settings.ShowCenterImage;
            _runImageAnimation = settings.RunImageAnimation;
            _intervalBetweenImages = settings.IntervalBetweenImages;
            _intervalForShowImage = settings.IntervalForShowImage;
            _changeThemeWhenDayIsChanged = settings.ChangeThemeWhenDayIsChanged;
            if (_themes == null)
            {
                _themes = new ObservableCollection<ThemeSettings>();
            }
            _themes.Clear();
            settings.Themes.ForEach(x => _themes.Add((ThemeSettings)x.Clone()));
            _monitor = settings.Monitor;
        }

        private ObservableCollection<ThemeSettings> _themes;
        public ObservableCollection<ThemeSettings> Themes
        {
            get { return _themes; }
            set { SetProperty(ref _themes, value); }
        }

        private bool _themeChecked;
        public bool ThemeChecked
        {
            get { return _themeChecked; }
            set { SetProperty(ref _themeChecked, value); }
        }

        private bool _pathChecked;
        public bool PathChecked
        {
            get { return _pathChecked; }
            set { SetProperty(ref _pathChecked, value); }
        }

        private string _imagesDirectoryName;
        public string ImagesDirectoryName
        {
            get { return _imagesDirectoryName; }
            set { SetProperty(ref _imagesDirectoryName, value); }
        }

        private string _imageFileName;
        public string ImageFileName
        {
            get { return _imageFileName; }
            set { SetProperty(ref _imageFileName, value); }
        }

        public string ImagesAbsoluteDirectoryName
        {
            get
            {
                return PathUtils.MakeAbsolutePath(_imagesDirectoryName, AssemblyUtils.AssemblyDirectoryName);
            }
        }

        public string ImageAbsoluteFileName
        {
            get
            {
                return PathUtils.MakeAbsolutePath(_imageFileName, AssemblyUtils.AssemblyDirectoryName);
            }
        }

        private bool _showCenterImage;
        public bool ShowCenterImage
        {
            get { return _showCenterImage; }
            set { SetProperty(ref _showCenterImage, value); }
        }

        private bool _runImageAnimation;
        public bool RunImageAnimation
        {
            get { return _runImageAnimation; }
            set { SetProperty(ref _runImageAnimation, value); }
        }

        private int _intervalBetweenImages;
        public int IntervalBetweenImages
        {
            get { return _intervalBetweenImages; }
            set { SetProperty(ref _intervalBetweenImages, value); }
        }

        private int _intervalForShowImage;
        public int IntervalForShowImage
        {
            get { return _intervalForShowImage; }
            set { SetProperty(ref _intervalForShowImage, value); }
        }

        private int? _monitor;
        public int? Monitor
        {
            get { return _monitor; }
            set { SetProperty(ref _monitor, value); }
        }

        private bool _changeThemeWhenDayIsChanged = false;
        public bool ChangeThemeWhenDayIsChanged
        {
            get { return _changeThemeWhenDayIsChanged; }
            set { SetProperty(ref _changeThemeWhenDayIsChanged, value); }
        }

        public IEnumerable<KeyValuePair<bool, string>> TrueFalseItems => new List<KeyValuePair<bool, string>>() { new KeyValuePair<bool, string>(true, "True"), new KeyValuePair<bool, string>(false, "False") };

        public IEnumerable<KeyValuePair<int?, string>> Monitors
        {
            get
            {
                var monitors = new List<KeyValuePair<int?, string>>() { new KeyValuePair<int?, string>(null, "All") };
                var monitor = 0;
                EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, (IntPtr hMonitor, IntPtr hdcMonitor, ref Common.Rect rect, IntPtr data) =>
                {
                    monitors.Add(new KeyValuePair<int?, string>(monitor, (monitor + 1).ToString()));
                    monitor++;
                    return true;
                }, IntPtr.Zero);
                return monitors;
            }
        }

        protected override void OnOK()
        {
            if (_themeChecked && Themes.Any() && Themes.All(x => !x.IsActive))
            {
                MessageBox.Show("You should select one theme.", "Warning", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_intervalBetweenImages <= 0)
            {
                MessageBox.Show("Interval between images must be greater than zero.", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_intervalForShowImage <= 0)
            {
                MessageBox.Show("Interval for show image must be greater than zero.", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            base.OnOK();
        }
    }
}