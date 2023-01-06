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

}
