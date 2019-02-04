using System;
using Prism.Mvvm;
using HolidayDesktop.Common;

namespace HolidayDesktop.Settings
{
    [Serializable]
    public class ThemeSettings : BindableBase, ICloneable
    {
        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set { SetProperty(ref _isActive, value); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _directoryName;
        public string DirectoryName
        {
            get { return _directoryName; }
            set { SetProperty(ref _directoryName, value); }
        }

        private DateTime? _date;
        public DateTime? Date
        {
            get { return _date; }
            set { SetProperty(ref _date, value); }
        }

        public string AbsoluteDirectoryName
        {
            get
            {                
                return PathUtils.MakeAbsolutePath(_directoryName, AssemblyUtils.AssemblyDirectoryName);
            }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
