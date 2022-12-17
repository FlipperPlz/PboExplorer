using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BisUtils.PBO;
using PboExplorer.Models;
using PboExplorer.Utils.Interfaces;
using PboExplorer.Utils.Managers;

namespace PboExplorer.Windows.PboExplorer
{
    /// <summary>
    /// Interaction logic for PboExplorerWindow.xaml
    /// </summary>
    public partial class PboExplorerWindow {
        private readonly EntryTreeManager TreeManager;
        private readonly ObservableCollection<IDocument> _documents = new();

        public PboExplorerWindow(PboFile pboFile) {
            InitializeComponent();
            TreeManager = new EntryTreeManager(pboFile);
            PboView.ItemsSource = TreeManager.EntryRoot.TreeChildren;

            _documents.CollectionChanged += OnDocumentsCollectionChanged;
            _documents.Add(new AboutEntry());
            DockManager.DocumentsSource =_documents;
        }

        private void SaveAs(object sender, RoutedEventArgs e) {
            throw new NotImplementedException();
            //TODO: This will be hard since we have a cache implemented 
        }
        
        private async void Save(object sender, RoutedEventArgs e) => 
            await TreeManager.DataRepository.SaveAllEditedEntries();

        private void Close(object sender, RoutedEventArgs e) {
            TreeManager.Dispose();
            Close();
        }

        private void CopySelectedEntryName(object sender, RoutedEventArgs e) {
            if(TreeManager.SelectedEntry is null) return;
            Clipboard.SetText(TreeManager.SelectedEntry.FullPath);
        }
        

        private async void CopySelectedEntryData(object sender, RoutedEventArgs e) {
            if(TreeManager.SelectedEntry is null) return;
            Clipboard.SetText(Encoding.UTF8.GetString((await TreeManager.GetCurrentEntryData()).ToArray()));
        }

        private void DeleteSelectedEntry(object sender, RoutedEventArgs e) {
            throw new NotImplementedException();
        }

        private void AddEntryWizard(object sender, RoutedEventArgs e) {
            throw new NotImplementedException();
        }
        
        //TODO: Bring focus to new tab
        private async Task ShowPboEntry(TreeDataEntry treeDataEntry) {
            TreeManager.SelectedEntry = treeDataEntry;
            var opened = _documents.Where(doc => doc.IsDocumentFor(treeDataEntry));

            if (!opened.Any()) {
                var text = Encoding.UTF8.GetString((await TreeManager.DataRepository.GetOrCreateEntryDataStream(treeDataEntry)).ToArray());
                var doc = new TextEntry(treeDataEntry ,text);
                _documents.Add(doc);
            }
            else {
                //TODO: Bring focus to opened file tab
            }
        }

        private void CanSave(object sender, CanExecuteRoutedEventArgs e) =>
            e.CanExecute = TreeManager.DataRepository.AreFilesEdited();

        private async void SearchButton_Click(object sender, RoutedEventArgs e) {
            var search = SearchBox.Text;
            if(string.IsNullOrWhiteSpace(search)) return;
            TreeManager.ClearSearchResults();
            SearchResultsView.ItemsSource = TreeManager.SearchResults;
            SearchButton.IsEnabled = false;
            TreeManager.SearchResults.Clear();
            foreach (var fileSearchResult in await TreeManager.SearchForString(search, false)) TreeManager.SearchResults.Add(fileSearchResult);
            SearchButton.IsEnabled = true;
        }

        private async void OnViewPboEntry(object sender, MouseButtonEventArgs e) {
            var fe = sender as FrameworkElement;
            switch (fe?.DataContext) {
                case TreeDataEntry treeDataEntry:
                    await ShowPboEntry(treeDataEntry);
                    break;
                default: return;
            }
        }

        //TODO: Consider handling the double click event
        private void ConfigView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
            throw new NotImplementedException();
        }

        private async void OnViewSearchResult(object sender, MouseButtonEventArgs e) {
            var fe = sender as FrameworkElement;
            switch (fe?.DataContext){
                case SearchResult searchResult:
                    await ShowPboEntry(searchResult.File);
                    break;
            }
        }

        private void OnDocumentsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var doc in e.NewItems?.Cast<IDocument>())
                    {
                        doc.CloseRequested += OnDocumentCloseRequested;
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var doc in e.OldItems?.Cast<IDocument>())
                    {
                        doc.CloseRequested -= OnDocumentCloseRequested;
                    }
                    break;
            }
        }

        private void OnDocumentCloseRequested(object? sender, EventArgs e)
        {
            if (sender is IDocument doc)
            {
                _documents.Remove(doc);
            }
        }
    }
}
