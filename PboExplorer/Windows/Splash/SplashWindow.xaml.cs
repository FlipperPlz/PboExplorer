using System;
using System.Windows;
using PboExplorer.Windows.Splash.Views.RecentFiles.ViewModels;

namespace PboExplorer.Windows.Splash
{
    /// <summary>
    /// Interaction logic for SplashWindow.xaml
    /// </summary>
    public partial class SplashWindow : Window
    {
        private SplashViewModel _viewModel;

        public SplashWindow()
        {
            InitializeComponent();
            _viewModel = new();
            _viewModel.CloseRequested += OnCloseRequested;
            DataContext = _viewModel;
        }

        private void OnCloseRequested(object? sender, EventArgs e)
        {
            _viewModel.CloseRequested -= OnCloseRequested;
            Close();
        }
    }
}
