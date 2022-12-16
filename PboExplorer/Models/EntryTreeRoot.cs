using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PboExplorer.Utils.Interfaces;
using PboExplorer.Utils.Managers;
using PboExplorer.Utils.Repositories;

namespace PboExplorer.Models; 

public class EntryTreeRoot : ITreeRoot {
    private readonly ObservableCollection<ITreeItem> _entryList = new();
    
    public IEnumerable<TreeDataEntry> Files => TreeChildren!.Where(s => s is TreeDataEntry).Cast<TreeDataEntry>();
    public IEnumerable<TreeDirectoryEntry> Directories => TreeChildren!.Where(s => s is TreeDirectoryEntry).Cast<TreeDirectoryEntry>();
    
    public EntryTreeManager TreeManager { get; set; }
    public EntryDataRepository DataRepository { get; set; }

    public EntryTreeRoot(EntryTreeManager treeManager) {
        TreeManager = treeManager;
        DataRepository = treeManager.DataRepository;
    }
    
    public ObservableCollection<ITreeItem>? TreeChildren {
        get =>  new(_entryList.ToImmutableSortedSet());
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

    public async Task<IEnumerable<FileSearchResult>> SearchForString(string search, bool cacheIfNotAlready) {
        var results = new List<FileSearchResult>();
        foreach (var dataEntry in RecursivelyGrabAllFiles()) 
            results.Add(await dataEntry.SearchForString(search, cacheIfNotAlready));
        return results;
    }
    
    public IEnumerable<TreeDataEntry> RecursivelyGrabAllFiles() => 
        Directories.SelectMany(d => d.RecursivelyGrabAllFiles()).Concat(Files);
    
}