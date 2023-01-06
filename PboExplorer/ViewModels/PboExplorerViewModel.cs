using CommunityToolkit.Mvvm.ComponentModel;
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

namespace PboExplorer.ViewModels;

public partial class PboExplorerViewModel : ObservableObject, IDisposable, IRecipient<ActivateDocumentMessage>
{
    private bool _disposed;
    [ObservableProperty]
    private IDocument? _activeDocument;

    public ObservableCollection<IDocument> Documents { get; } = new();
    public ObservableCollection<IPane> Panes { get; } = new();

    public PboExplorerViewModel(EntryTreeManager treeManager) // CONSIDER: inject pane vms directly
    {
        Documents.CollectionChanged += OnDocumentsCollectionChanged;
        Documents.Add(new AboutEntryViewModel());

        Panes.Add(new FileTreePaneViewModel(treeManager));
        Panes.Add(new ConfigTreePaneViewModel());
        Panes.Add(new PboMetadataPaneViewModel());
        Panes.Add(new SearchResultsPaneViewModel());
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
