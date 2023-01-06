using System;
using System.Text;
using System.Windows;
using System.Windows.Input;
using BisUtils.PBO;
using CommunityToolkit.Mvvm.Messaging;
using PboExplorer.Messages;
using PboExplorer.Models;
using PboExplorer.Utils.Managers;
using PboExplorer.ViewModels;

namespace PboExplorer.Windows.PboExplorer
{
    /// <summary>
    /// Interaction logic for PboExplorerWindow.xaml
    /// </summary>
    public partial class PboExplorerWindow {
        private readonly EntryTreeManager TreeManager; // TODO: Move to singleton service

        public PboExplorerWindow(PboFile pboFile) {
            InitializeComponent();
            TreeManager = new EntryTreeManager(pboFile);

            DataContext = new PboExplorerViewModel(TreeManager);
        }

        private void SaveAs(object sender, RoutedEventArgs e) {
            throw new NotImplementedException();
            //TODO: This will be hard since we have a cache implemented 
        }

        // TODO: Move to appropriate VM
        private async void Save(object sender, RoutedEventArgs e) => 
            await TreeManager.DataRepository.SaveAllEditedEntries();

        // TODO: Move to appropriate VM
        private void Close(object sender, RoutedEventArgs e) {
            TreeManager.Dispose();
            Close();
        }

        // TODO: Move to FileTreePane VM
        private void CopySelectedEntryName(object sender, RoutedEventArgs e) {
            if(TreeManager.SelectedEntry is null) return;
            Clipboard.SetText(TreeManager.SelectedEntry.FullPath);
        }

        // TODO: Move to FileTreePane VM
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
        
        // TODO: Move to appropriate VM
        private void CanSave(object sender, CanExecuteRoutedEventArgs e) =>
            e.CanExecute = TreeManager.DataRepository.AreFilesEdited();
        
        // TODO: Move to appropriate VM (possibly PboExplorerViewModel)
        private async void SearchButton_Click(object sender, RoutedEventArgs e) {
            var search = SearchBox.Text;
            if(string.IsNullOrWhiteSpace(search)) return;
            TreeManager.ClearSearchResults();
            //SearchResultsView.ItemsSource = TreeManager.SearchResults;
            SearchButton.IsEnabled = false;
            TreeManager.SearchResults.Clear();
            foreach (var fileSearchResult in await TreeManager.SearchForString(search, false)) TreeManager.SearchResults.Add(fileSearchResult);
            SearchButton.IsEnabled = true;
        }
        
        // TODO: Move to FileTreePane VM
        private void OnViewPboEntry(object sender, MouseButtonEventArgs e) {
            var fe = sender as FrameworkElement;
            switch (fe?.DataContext) {
                case TreeDataEntry treeDataEntry:
                    WeakReferenceMessenger.Default.Send(new ActivateDocumentMessage(treeDataEntry));
                    break;
                default: return;
            }
        }

        // TODO: Move to SearchResultsPane VM
        private void OnViewSearchResult(object sender, MouseButtonEventArgs e) {
            var fe = sender as FrameworkElement;
            switch (fe?.DataContext){
                case SearchResult searchResult:
                    WeakReferenceMessenger.Default.Send(new ActivateDocumentMessage(searchResult.File));
                    break;
            }
        }
    }
}
