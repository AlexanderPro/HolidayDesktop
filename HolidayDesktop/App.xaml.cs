using System;
using System.Threading;
using System.ComponentModel;
using System.Windows;
using HolidayDesktop.Common;

namespace HolidayDesktop
{
    public partial class App : Application
    {
        private Mutex _oneInstanceMutex;
        private ViewManager _manager;

        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _oneInstanceMutex = new Mutex(false, "HolidayDesktopOneInstanceMutex", out var createNew);
            if (!createNew)
            {
                Shutdown();
                return;
            }

            _manager = new ViewManager();
            _manager.LoadSettings();
            _manager.CreateWindows();
            _manager.InitTray();
            _manager.Start();
        }

        private void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            exception = exception ?? new Exception("OnCurrentDomainUnhandledException");
            var message = exception.Message;
            if (exception is Win32Exception)
            {
                message = $"Win32 Error Code = {((Win32Exception)exception).ErrorCode},{Environment.NewLine}{message}";
            }
            MessageBox.Show(message, AssemblyUtils.AssemblyProductName, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
