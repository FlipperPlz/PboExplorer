using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PboExplorer.Utils.Interfaces;
using PboExplorer.Utils.Managers;
using PboExplorer.Utils.Repositories;

namespace PboExplorer.Models; 

public class TreeDirectoryEntry : ITreeEnumerableItem {
    public string Title { get; set; }
    public string Description { get; set; }
    public EntryTreeManager TreeManager { get; set; }
    public EntryDataRepository DataRepository { get; set; }
    public ITreeRoot TreeRoot { get; set; }
    public ITreeEnumerable TreeParent { get; set; }
    public IEnumerable<TreeDataEntry> Files => ((ITreeEnumerable)this).Files;
    public IEnumerable<TreeDirectoryEntry> Directories => ((ITreeEnumerable)this).Directories;


    private readonly ObservableCollection<ITreeItem> _entryList = new();
    
    
    public TreeDirectoryEntry(EntryTreeManager treeManager) {
        TreeManager = treeManager;
        DataRepository = treeManager.DataRepository;
        TreeRoot = treeManager.EntryRoot;
    }
    
    public ICollection<ITreeItem>? TreeChildren {
        get => _entryList;
        set {
            _entryList.Clear();
            if (value == null) return;
            foreach (var treeItem in value)
                _entryList.Add(treeItem);
        }
    }

    
    
    
    public ITreeItem AddChild(ITreeItem child) {
        _entryList.Add(child);
        
        return child;
    }

    public void RemoveChild(ITreeItem child) {
        if(!_entryList.Contains(child)) return;
        
        _entryList.Remove(child);
    }

    public IEnumerable<TreeDataEntry> RecursivelyGrabAllFiles() => ((ITreeEnumerable)this).RecursivelyGrabAllFiles();
}