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

public class TreeDirectoryEntry : ITreeEnumerableItem, IComparable<TreeDirectoryEntry> {
    public string Title { get; set; }
    public string Description { get; set; }
    public EntryTreeManager TreeManager { get; set; }
    public EntryDataRepository DataRepository { get; set; }
    public ITreeRoot TreeRoot { get; set; }
    public ITreeEnumerable TreeParent { get; set; }
    public IEnumerable<TreeDataEntry> Files => TreeChildren!.Where(s => s is TreeDataEntry).Cast<TreeDataEntry>();
    public bool CurrentlySearching { get; private set; }

    public IEnumerable<TreeDirectoryEntry> Directories =>
        TreeChildren!.Where(s => s is TreeDirectoryEntry).Cast<TreeDirectoryEntry>();


    private readonly ObservableCollection<ITreeItem> _entryList = new();
    
    public TreeDirectoryEntry(EntryTreeManager treeManager) {
        TreeManager = treeManager;
        DataRepository = treeManager.DataRepository;
        TreeRoot = treeManager.EntryRoot;
    }

    public ObservableCollection<ITreeItem> TreeChildren {
        get => new (_entryList.ToImmutableSortedSet());
        set {
            _entryList.Clear();
            foreach (var treeItem in value)
                _entryList.Add(treeItem);
        }
    }

    public async Task<IEnumerable<FileSearchResult>> SearchForString(string search, bool cacheIfNotAlready) {
        var results = new List<FileSearchResult>();
        if (CurrentlySearching) return results;
        CurrentlySearching = true;
        foreach (var dataEntry in RecursivelyGrabAllFiles())
            results.Add(await dataEntry.SearchForString(search, cacheIfNotAlready));
        CurrentlySearching = false;
        return results;
    }

    public ITreeItem AddChild(ITreeItem child) {
        _entryList.Add(child);

        return child;
    }

    public void RemoveChild(ITreeItem child) {
        if (!_entryList.Contains(child)) return;

        _entryList.Remove(child);
    }

    public IEnumerable<TreeDataEntry> RecursivelyGrabAllFiles() =>
        Directories.SelectMany(d => d.RecursivelyGrabAllFiles()).Concat(Files);

    public int CompareTo(TreeDirectoryEntry? other) {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        var titleComparison = string.Compare(Title, other.Title, StringComparison.Ordinal);
        return titleComparison != 0 ? titleComparison : string.Compare(Description, other.Description, StringComparison.Ordinal);
    }

    public int CompareTo(ITreeItem? other) {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return other switch {
            TreeDirectoryEntry treeDirectoryEntry => CompareTo(treeDirectoryEntry),
            TreeDataEntry => -1,
            _ => 0
        };
    }
}