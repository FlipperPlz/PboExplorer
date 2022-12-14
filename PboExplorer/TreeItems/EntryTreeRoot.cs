using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using PboExplorer.Utils;

namespace PboExplorer.TreeItems; 

public class EntryTreeRoot : ITreeRoot {
    private readonly ObservableCollection<ITreeItem> _entryList = new();
    
    public IEnumerable<TreeDataEntry> Files => _entryList.Where(s => s is TreeDataEntry).Cast<TreeDataEntry>();
    public IEnumerable<TreeDirectoryEntry> Directories => _entryList.Where(s => s is TreeDirectoryEntry).Cast<TreeDirectoryEntry>();
    
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

    public IEnumerable<TreeDataEntry> RecursivelyGrabAllFiles() => 
        Directories.SelectMany(d => d.RecursivelyGrabAllFiles()).Concat(Files);
    
    public T GetOrCreateChild<T>(string title) where T : ITreeItem {
        var folders = title.Split(Path.DirectorySeparatorChar).Where(s => !string.IsNullOrEmpty(s)).ToList();
        if (!folders.Any()) {
            folders.Add("PboExplorer");
            folders.Add($"DisfiguredEntry.{Guid.NewGuid()}");
        }
        switch (typeof(T).Name) {
            case nameof(TreeDirectoryEntry): {
                if (string.IsNullOrWhiteSpace(title)) throw new Exception();
                var found = Directories.FirstOrDefault(d => string.Equals(d.Title, folders.First()));
                if (found != null) return found.GetOrCreateChild<T>(string.Join(Path.DirectorySeparatorChar, folders.Skip(1)));
                found = new TreeDirectoryEntry {
                    Title = folders.First(),
                    TreeParent = this,
                    TreeRoot = this,
                    
                };
                AddChild(found);
                var nextPaths = folders.Skip(1).ToList();
                return found.GetOrCreateChild<T>(string.Join(Path.DirectorySeparatorChar, nextPaths));
            }
            case nameof(TreeDataEntry): {
                if (folders.Count == 1) {
                    var foundDataEntry = Files.FirstOrDefault(d => string.Equals(d.Title, folders.First()));
                    if (foundDataEntry is not null) return (T) (ITreeItem) foundDataEntry;
                    
                    var returnFile = new TreeDataEntry() {
                        Title = folders.First(),
                        TreeParent = this,
                        TreeRoot = this
                    };
                    AddChild(returnFile);
                    return (T) (ITreeItem) returnFile;
                }
                var found = Directories.FirstOrDefault(d => string.Equals(d.Title, folders.First()));
                if (found != null) return found.GetOrCreateChild<T>(string.Join(Path.DirectorySeparatorChar, folders.Skip(1)));
                found = new TreeDirectoryEntry {
                    Title = folders.First(),
                    TreeParent = this,
                    TreeRoot = this
                };
                AddChild(found);
                var nextPaths = folders.Skip(1).ToList();
                return found.GetOrCreateChild<T>(string.Join(Path.DirectorySeparatorChar, nextPaths));
            }
            default: throw new NotSupportedException();
        }
    }

}