using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PboExplorer.Messages;
using PboExplorer.Models;
using PboExplorer.Utils.Managers;
using System.Collections.ObjectModel;

namespace PboExplorer.ViewModels.Panes;

public partial class SearchResultsPaneViewModel : PaneViewModel
{
    private readonly EntryTreeManager _treeManager;

    public override string Title => "Search Results";
    public ObservableCollection<FileSearchResult> Results => _treeManager.SearchResults;

    public SearchResultsPaneViewModel(EntryTreeManager treeManager)
    {
        _treeManager = treeManager;
    }

    [RelayCommand]
    public void OpenPreview(object item)
    {
        if (item is SearchResult result)
        {
            WeakReferenceMessenger.Default.Send(new ActivateDocumentMessage(result.File));
        }
    }
}
