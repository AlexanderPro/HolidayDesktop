using System.Windows;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using HolidayDesktop.Common;
using HolidayDesktop.Views;
using HolidayDesktop.Settings;

namespace HolidayDesktop.ViewModels
{
    sealed class TaskbarIconViewModel : BindableBase
    {
        private readonly ViewManager _manager;
        private readonly IDialogService _dialogService;
        private readonly ProgramSettings _settings;

        public TaskbarIconViewModel(ViewManager manager, ProgramSettings settings, IDialogService dialogService)
        {
            _manager = manager;
            _dialogService = dialogService;
            _settings = settings;

            ExitCommand = new DelegateCommand(() => Application.Current.Shutdown());

            SettingsCommand = new DelegateCommand(() =>
            {
                var dialog = _dialogService.CreateDialog<SettingsViewModel, SettingsView>(_manager.Settings);
                if (dialog.ShowDialog() == true)
                {
                    manager.ApplySettings(dialog);
                }
            });

            AboutCommand = new DelegateCommand(() =>
            {
                var dialog = _dialogService.CreateDialog<AboutViewModel, AboutView>();
                dialog.ShowDialog();
            });

            StartStopCommand = new DelegateCommand(() =>
            {
                if (manager.IsWindowVisible)
                {
                    manager.Stop();
                }
                else
                {
                    manager.Start();
                }
                RaisePropertyChanged(nameof(MenuItemStartStopText));
                RaisePropertyChanged(nameof(MenuItemStartStopIcon));
            });

            AutoStartCommand = new DelegateCommand(() =>
            {
                if (StartUpManager.IsInStartup())
                {
                    StartUpManager.RemoveFromStartup();
                }
                else
                {
                    StartUpManager.AddToStartup();
                }
                RaisePropertyChanged(nameof(AutoStart));
            });
        }

        public string MenuItemStartStopText => _manager.IsWindowVisible ? "Stop" : "Start";
        public bool AutoStart => StartUpManager.IsInStartup();
        public string MenuItemStartStopIcon => _manager.IsWindowVisible ? "pack://application:,,,/Icons/Stop.ico" : "pack://application:,,,/Icons/Play.ico";

        public ICommand StartStopCommand { get; }
        public ICommand AutoStartCommand { get; }
        public ICommand SettingsCommand { get; }
        public ICommand AboutCommand { get; }
        public ICommand ExitCommand { get; }
    }
}
