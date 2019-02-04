using System.IO;
using System.Windows;
using System.Windows.Controls;
using HolidayDesktop.Common;
using HolidayDesktop.Settings;
using HolidayDesktop.ViewModels;

namespace HolidayDesktop.Views
{
    public partial class SettingsView : Window
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void BrowseImagesDirectory_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (SettingsViewModel)DataContext;
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (!string.IsNullOrEmpty(viewModel.ImagesAbsoluteDirectoryName) && Directory.Exists(viewModel.ImagesAbsoluteDirectoryName))
            {
                dialog.SelectedPath = PathUtils.MakeAbsolutePath(viewModel.ImagesAbsoluteDirectoryName, AssemblyUtils.AssemblyDirectoryName);
            }
            else
            {
                dialog.SelectedPath = AssemblyUtils.AssemblyDirectoryName;
            }
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                viewModel.ImagesDirectoryName = PathUtils.MakeRelativePath(dialog.SelectedPath, AssemblyUtils.AssemblyDirectoryName);
            }
        }

        private void BrowseImageFile_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (SettingsViewModel)DataContext;
            var dialog = new System.Windows.Forms.OpenFileDialog();
            if (!string.IsNullOrEmpty(viewModel.ImageAbsoluteFileName) && File.Exists(viewModel.ImageAbsoluteFileName))
            {
                dialog.InitialDirectory = Path.GetDirectoryName(PathUtils.MakeAbsolutePath(viewModel.ImageAbsoluteFileName, AssemblyUtils.AssemblyDirectoryName));
            }
            else
            {
                dialog.InitialDirectory = AssemblyUtils.AssemblyDirectoryName;
            }
            dialog.Filter = $"Image files ({string.Join(";", viewModel.Settings.ImageFileExtensions)})|{string.Join(";", viewModel.Settings.ImageFileExtensions)}|All files (*.*)|*.*";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                viewModel.ImageFileName = PathUtils.MakeRelativePath(dialog.FileName, AssemblyUtils.AssemblyDirectoryName);
            }
        }

        private void AddTheme_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            var themeDirectoryName = Path.Combine(AssemblyUtils.AssemblyDirectoryName, "Themes");
            dialog.SelectedPath = Directory.Exists(themeDirectoryName) ? themeDirectoryName : AssemblyUtils.AssemblyDirectoryName;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var theme = new ThemeSettings
                {
                    IsActive = true,
                    DirectoryName = PathUtils.MakeRelativePath(dialog.SelectedPath, AssemblyUtils.AssemblyDirectoryName),
                    Name = Path.GetFileName(dialog.SelectedPath.TrimEnd('\\')),
                    Date = null
                };

                var viewModel = (SettingsViewModel)DataContext;
                foreach (var viewModelTheme in viewModel.Themes)
                {
                    viewModelTheme.IsActive = false;
                }
                viewModel.Themes.Add(theme);

                var imagesDirectoryName = Path.Combine(dialog.SelectedPath, "Images");
                if (!Directory.Exists(imagesDirectoryName))
                {
                    MessageBox.Show("The theme must contain \"Images\" subfolder.", "Warning", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void RemoveTheme_Click(object sender, RoutedEventArgs e)
        {
            var theme = (ThemeSettings)((Button)sender).DataContext;
            var viewModel = (SettingsViewModel)DataContext;
            viewModel.Themes.Remove(theme);
        }

        private void ChangeTheme_Click(object sender, RoutedEventArgs e)
        {
            var theme = (ThemeSettings)((Button)sender).DataContext;
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            var themeDirectoryName = Path.Combine(AssemblyUtils.AssemblyDirectoryName, "Themes");
            dialog.SelectedPath = PathUtils.MakeAbsolutePath(theme.DirectoryName, AssemblyUtils.AssemblyDirectoryName);
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                theme.Name = Path.GetFileName(dialog.SelectedPath.TrimEnd('\\'));
                theme.DirectoryName = PathUtils.MakeRelativePath(dialog.SelectedPath, AssemblyUtils.AssemblyDirectoryName);
                var imagesDirectoryName = Path.Combine(dialog.SelectedPath, "Images");
                if (!Directory.Exists(imagesDirectoryName))
                {
                    MessageBox.Show("The theme must contain \"Images\" subfolder.", "Warning", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}
