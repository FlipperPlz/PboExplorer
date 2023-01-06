using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PboExplorer.Messages;
using PboExplorer.Utils.Interfaces;
using PboExplorer.Utils.Managers;
using PboExplorer.ViewModels.Panes;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PboExplorer.ViewModels;

public partial class PboExplorerViewModel : ObservableObject, IDisposable, IRecipient<ActivateDocumentMessage>
{
    private readonly EntryTreeManager _treeManager;
    private bool _disposed;

    [ObservableProperty]
    private IDocument? _activeDocument;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SearchCommand))]
    private string? _searchTerm;

    public ObservableCollection<IDocument> Documents { get; } = new();
    public ObservableCollection<IPane> Panes { get; } = new();

    public event EventHandler? ExitRequested;

    public PboExplorerViewModel(EntryTreeManager treeManager)
    {
        _treeManager = treeManager;

        Documents.CollectionChanged += OnDocumentsCollectionChanged;
        Documents.Add(new AboutEntryViewModel());

        Panes.Add(new FileTreePaneViewModel(treeManager));
        Panes.Add(new ConfigTreePaneViewModel());
        Panes.Add(new PboMetadataPaneViewModel());
        Panes.Add(new SearchResultsPaneViewModel(treeManager));
        Panes.Add(new EntryInformationPaneViewModel());

        WeakReferenceMessenger.Default.Register(this);
    }

    public async void Receive(ActivateDocumentMessage message)
    {
        var treeManager = message.Data.TreeManager; // TODO: Consider refactoring 

        treeManager.SelectedEntry = message.Data;
        var opened = Documents.Where(doc => doc.IsDocumentFor(message.Data));

        if (!opened.Any())
        {
            var text = Encoding.UTF8.GetString((await treeManager.DataRepository.GetOrCreateEntryDataStream(message.Data)).ToArray());  // TODO: Consider refactoring 
            var doc = new TextEntryViewModel(message.Data, text);
            Documents.Add(doc);
            ActiveDocument = doc;
        }
        else
        {
            ActiveDocument = opened.FirstOrDefault();
        }
    }
    
    [RelayCommand]
    public void Exit() => ExitRequested?.Invoke(_searchTerm, EventArgs.Empty);
    
    public bool CanSearch(string? term) => !string.IsNullOrWhiteSpace(term);

    [RelayCommand(CanExecute = nameof(CanSearch))]
    public async Task Search(string? term)
    {
        if (!string.IsNullOrWhiteSpace(term))
        {
            // TODO: Encapsulate this in search service
            _treeManager.SearchResults.Clear();
            foreach (var fileSearchResult in await _treeManager.SearchForString(term, false))
            {
                _treeManager.SearchResults.Add(fileSearchResult);
            }
        }
    }

    [RelayCommand]
    public async Task Save()
    {
        await _treeManager.DataRepository.SaveAllEditedEntries();
    }

    [RelayCommand]
    public async Task SaveAs()
    {
        throw new NotImplementedException();
        //TODO: This will be hard since we have a cache implemented 
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
            Documents.Remove(doc);
        }
    }

    #region IDisposable
    ~PboExplorerViewModel() => Dispose(false);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        Documents.CollectionChanged -= OnDocumentsCollectionChanged;

        _disposed = true;
    }

    #endregion
}
