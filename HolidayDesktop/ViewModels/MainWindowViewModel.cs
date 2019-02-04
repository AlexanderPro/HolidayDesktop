using System.Windows;
using Prism.Mvvm;
using HolidayDesktop.Common;
using HolidayDesktop.Settings;

namespace HolidayDesktop.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        public ProgramSettings Settings { get; }

        public MainWindowViewModel(ProgramSettings settings)
        {
            Settings = settings;
            _imageFileName = settings.ImageFileName;
        }

        public Visibility MainImageVisibility => Settings.ShowCenterImage ? Visibility.Visible : Visibility.Collapsed;

        private string _imageFileName;
        public string ImageFileName
        {
            get { return _imageFileName; }
            set { SetProperty(ref _imageFileName, value); }
        }

        public string ImageAbsoluteFileName
        {
            get
            {
                return PathUtils.MakeAbsolutePath(_imageFileName, AssemblyUtils.AssemblyDirectoryName);
            }
        }

        public void Refresh()
        {
            RaisePropertyChanged(nameof(ImageFileName));
            RaisePropertyChanged(nameof(ImageAbsoluteFileName));
        }
    }
}