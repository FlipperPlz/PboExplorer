using System;
using System.Text;
using System.Windows;
using System.Windows.Input;
using BisUtils.PBO;
using PboExplorer.Utils.Managers;
using PboExplorer.ViewModels;

namespace PboExplorer.Windows.PboExplorer
{
    /// <summary>
    /// Interaction logic for PboExplorerWindow.xaml
    /// </summary>
    public partial class PboExplorerWindow {
        private readonly EntryTreeManager TreeManager; // TODO: Move to singleton service
        private readonly PboExplorerViewModel _viewModel;
        
        public PboExplorerWindow(PboFile pboFile) {
            InitializeComponent();
            TreeManager = new EntryTreeManager(pboFile);

            _viewModel= new PboExplorerViewModel(TreeManager);
            _viewModel.ExitRequested += OnExitRequested;
            DataContext = _viewModel;
        }

        private void OnExitRequested(object? sender, EventArgs e)
        {
            _viewModel.ExitRequested -= OnExitRequested;
            TreeManager.Dispose();
            Close();
        }

        // TODO: Move to FileTreePane VM
        private void CopySelectedEntryName(object sender, RoutedEventArgs e) {
            if(TreeManager.SelectedEntry is null) return;
            Clipboard.SetText(TreeManager.SelectedEntry.FullPath);
        }

        // TODO: Move to FileTreePane Or Entry VM
        private async void CopySelectedEntryData(object sender, RoutedEventArgs e) {
            if(TreeManager.SelectedEntry is null) return;
            Clipboard.SetText(Encoding.UTF8.GetString((await TreeManager.GetCurrentEntryData()).ToArray()));
        }

        // TODO: Move to FileTreePane VM
        private void DeleteSelectedEntry(object sender, RoutedEventArgs e) {
            throw new NotImplementedException();
        }

        // TODO: Move to appropriate VM
        private void AddEntryWizard(object sender, RoutedEventArgs e) {
            throw new NotImplementedException();
        }
    }
}
