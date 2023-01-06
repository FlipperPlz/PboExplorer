using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PboExplorer.Messages;
using PboExplorer.Models;
using PboExplorer.Utils.Interfaces;
using PboExplorer.Utils.Managers;
using System.Collections.ObjectModel;

namespace PboExplorer.ViewModels.Panes;

public partial class FileTreePaneViewModel : PaneViewModel
{
    private readonly EntryTreeManager _treeManager;

    public override string Title => "Pbo Explorer";

    public ObservableCollection<ITreeItem> Entries => _treeManager.EntryRoot.TreeChildren;

    public FileTreePaneViewModel(EntryTreeManager treeManager)
    {
        _treeManager = treeManager;
    }

    [RelayCommand]
    public void OpenPreview(ITreeItem item)
    {
        if (item is TreeDataEntry entry)
        {
            WeakReferenceMessenger.Default.Send(new ActivateDocumentMessage(entry));
        }
    }
}
