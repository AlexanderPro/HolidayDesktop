using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Mvvm;
using HolidayDesktop.Common;

namespace HolidayDesktop.Settings
{
    [Serializable]
    public class ProgramSettings : BindableBase
    {
        private string _imagesDirectoryName = "";
        public string ImagesDirectoryName
        {
            get { return _imagesDirectoryName; }
            set { SetProperty(ref _imagesDirectoryName, value); }
        }

        private string _imageFileName = "";
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

        private List<string> _imageFileExtensions = new List<string> { "*.bmp", "*.jpg", "*.jpeg", "*.png", "*.gif", "*.tiff" };
        public List<string> ImageFileExtensions
        {
            get { return _imageFileExtensions.Distinct().ToList(); }
            set { SetProperty(ref _imageFileExtensions, value); }
        }

        private bool _showCenterImage = true;
        public bool ShowCenterImage
        {
            get { return _showCenterImage; }
            set { SetProperty(ref _showCenterImage, value); }
        }

        private bool _runImageAnimation = true;
        public bool RunImageAnimation
        {
            get { return _runImageAnimation; }
            set { SetProperty(ref _runImageAnimation, value); }
        }

        private int _intervalBetweenImages = 3;
        public int IntervalBetweenImages
        {
            get { return _intervalBetweenImages; }
            set { SetProperty(ref _intervalBetweenImages, value); }
        }

        private int _intervalForShowImage = 10;
        public int IntervalForShowImage
        {
            get { return _intervalForShowImage; }
            set { SetProperty(ref _intervalForShowImage, value); }
        }

        private int? _monitor = null;
        public int? Monitor
        {
            get { return _monitor; }
            set { SetProperty(ref _monitor, value); }
        }

        private ViewType _viewType = ViewType.SimplePath;
        public ViewType ViewType
        {
            get { return _viewType; }
            set { SetProperty(ref _viewType, value); }
        }

        private List<ThemeSettings> _themes = new List<ThemeSettings>() { };
        public List<ThemeSettings> Themes
        {
            get { return _themes; }
            set { SetProperty(ref _themes, value); }
        }

        private bool _changeThemeWhenDayIsChanged = false;
        public bool ChangeThemeWhenDayIsChanged
        {
            get { return _changeThemeWhenDayIsChanged; }
            set { SetProperty(ref _changeThemeWhenDayIsChanged, value); }
        }
    }
}
