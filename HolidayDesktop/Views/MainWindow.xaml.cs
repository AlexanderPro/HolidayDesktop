using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.IO;
using System.Linq;
using HolidayDesktop.Common;
using HolidayDesktop.Animation;
using HolidayDesktop.ViewModels;
using static HolidayDesktop.Common.NativeConstants;

namespace HolidayDesktop.Views
{
    public partial class MainWindow : Window
    {
        private AnimationEngine _animationEngine;

        public bool AllowClose { get; set; } = false;

        public MainWindow()
        {
            InitializeComponent();
            _animationEngine = new AnimationEngine(canvas, TimeSpan.FromMilliseconds(int.MaxValue), TimeSpan.FromMilliseconds(int.MaxValue));
        }

        public void StartAnimation()
        {
            var viewModel = (MainWindowViewModel)DataContext;
            if (viewModel.Settings.ViewType == Settings.ViewType.SimplePath && viewModel.Settings.RunImageAnimation && Directory.Exists(viewModel.Settings.ImagesAbsoluteDirectoryName))
            {
                var imagesDirectoryInfo = new DirectoryInfo(viewModel.Settings.ImagesAbsoluteDirectoryName);
                _animationEngine.FileNames = imagesDirectoryInfo.GetFilesByExtensions(viewModel.Settings.ImageFileExtensions.Select(x => x.Replace("*", "")).ToArray()).Select(x => x.FullName).ToList();
                if (_animationEngine.FileNames.Any())
                {
                    _animationEngine.FileNames.Shuffle();
                    _animationEngine.IntervalBetweenImages = TimeSpan.FromSeconds(viewModel.Settings.IntervalBetweenImages);
                    _animationEngine.IntervalForShowImage = TimeSpan.FromSeconds(viewModel.Settings.IntervalForShowImage);
                    _animationEngine.Start();
                }
            }
            else if (viewModel.Settings.ViewType == Settings.ViewType.Theme)
            {
                var theme = viewModel.Settings.Themes.FirstOrDefault(x => x.IsActive);
                if (theme != null && Directory.Exists(theme.AbsoluteDirectoryName))
                {
                    var themeDirectoryInfo = new DirectoryInfo(theme.AbsoluteDirectoryName);
                    var imageFileName = themeDirectoryInfo.GetFilesByExtensions(viewModel.Settings.ImageFileExtensions.Select(x => x.Replace("*", "")).ToArray()).Select(x => x.FullName).FirstOrDefault() ?? "";
                    viewModel.ImageFileName = imageFileName == "" ? "" : PathUtils.MakeRelativePath(imageFileName, AssemblyUtils.AssemblyDirectoryName);
                    viewModel.Refresh();
                    var imagesDirectoryName = Path.Combine(theme.AbsoluteDirectoryName, "Images");
                    if (viewModel.Settings.RunImageAnimation && Directory.Exists(imagesDirectoryName))
                    {
                        var imagesDirectoryInfo = new DirectoryInfo(imagesDirectoryName);
                        _animationEngine.FileNames = imagesDirectoryInfo.GetFilesByExtensions(viewModel.Settings.ImageFileExtensions.Select(x => x.Replace("*", "")).ToArray()).Select(x => x.FullName).ToList();
                        if (_animationEngine.FileNames.Any())
                        {
                            _animationEngine.FileNames.Shuffle();
                            _animationEngine.IntervalBetweenImages = TimeSpan.FromSeconds(viewModel.Settings.IntervalBetweenImages);
                            _animationEngine.IntervalForShowImage = TimeSpan.FromSeconds(viewModel.Settings.IntervalForShowImage);
                            _animationEngine.Start();
                        }
                    }
                }
                else
                {
                    viewModel.ImageFileName = "";
                    viewModel.Refresh();
                }
            }
        }

        public void StopAnimation()
        {
            _animationEngine.Stop();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var handle = new WindowInteropHelper(this).Handle;
            var hwndSource = HwndSource.FromHwnd(handle);
            hwndSource.AddHook(WindowProc);
            WindowUtils.SetStyles(handle);
            WindowUtils.ShowAlwaysOnDesktop(handle);
            WindowUtils.EnableNoActive(handle, true);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !AllowClose;
        }

        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_WINDOWPOSCHANGING)
            {
                var windowPos = (WindowPos)Marshal.PtrToStructure(lParam, typeof(WindowPos));
                windowPos.hwndInsertAfter = new IntPtr(HWND_BOTTOM);
                windowPos.flags &= ~(uint)SWP_NOZORDER;
                handled = true;
            }
            return IntPtr.Zero;
        }
    }
}
