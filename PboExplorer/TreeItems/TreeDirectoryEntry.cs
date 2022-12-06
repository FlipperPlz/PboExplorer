using System;
using System.Collections.Generic;
using System.Linq;
using PboExplorer.Utils;
using Path = System.IO.Path;
using StringBuilder = System.Text.StringBuilder;

namespace PboExplorer.TreeItems; 

public class TreeDirectoryEntry : ITreeItem {
    private string _directoryName;
    
    public IEnumerable<TreeDirectoryEntry> ChildDirectories => TreeChildren!.Where(s => s is TreeDirectoryEntry).Cast<TreeDirectoryEntry>();
    public IEnumerable<TreeDataEntry> ChildFiles => TreeChildren!.Where(s => s is TreeDataEntry).Cast<TreeDataEntry>();
    
    public string TreeTitle {
        get => _directoryName;
        set {
            _directoryName = value;

            foreach (var file in RecursivelyGrabAllFiles) file.TreePath = file.TreePath;
        }
    }

    public string TreePath {
        get {
            var pathBuilder = new StringBuilder();
            if (TreeParent is not null) pathBuilder.Append(TreeParent.TreePath).Append(Path.DirectorySeparatorChar);
            pathBuilder.Append(TreeTitle).Append(Path.DirectorySeparatorChar);
            return pathBuilder.ToString();
        }
        set => throw new NotImplementedException();
    }

    public ICollection<ITreeItem>? TreeChildren { get; set; }
    public ITreeItem? TreeParent { get; set; }


    public TreeDirectoryEntry(string name, ITreeItem? treeParent = null) {
        _directoryName = name;
        TreeParent = treeParent; 
    }
    
    public void AddEntry(TreeDataEntry entry) {
        TreeChildren ??= new List<ITreeItem>();
        
        TreeChildren.Add(entry);
    }
    
    public TreeDirectoryEntry GetOrCreateDirectory(string childName) {
        TreeChildren ??= new List<ITreeItem>();
        if (childName == string.Empty) return this;
        var folders = childName.Split(Path.DirectorySeparatorChar).Where(s => !string.IsNullOrEmpty(s));
        
        var found = ChildDirectories.FirstOrDefault(d => string.Equals(d._directoryName, folders.First()));
        if (found != null) return found.GetOrCreateDirectory(string.Join(Path.DirectorySeparatorChar, folders.Skip(1)));
        found = new TreeDirectoryEntry(folders.First());
        TreeChildren.Add(found);

        var nextPaths = folders.Skip(1).ToList();
        
        return nextPaths.Count != 0 ? found.GetOrCreateDirectory(string.Join(Path.DirectorySeparatorChar, nextPaths)) : found;
    }

    public IEnumerable<ITreeItem> RecursivelyGrabAllFiles => ChildDirectories.SelectMany(d => d.RecursivelyGrabAllFiles).Concat(ChildFiles);
    
}