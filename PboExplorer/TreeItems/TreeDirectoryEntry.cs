using System.Collections.Generic;
using System.Linq;
using PboExplorer.Utils;

namespace PboExplorer.TreeItems; 

public class TreeDirectoryEntry : ITreeItem {
    public readonly List<TreeDirectoryEntry> ChildrenDirectories = new();
    private readonly List<TreeDataEntry> ChildrenFiles = new();
    private List<ITreeItem>? _children;
    
    public string DirectoryName { get; set; }
    
    public TreeDirectoryEntry(string name) => DirectoryName = name;

    public ICollection<ITreeItem>? Children => GetChildren();
    public string Name => DirectoryName;
    
    
    public string GetTreeName() => DirectoryName;
    
    public void AddEntry(TreeDataEntry entry) {
        ChildrenFiles.Add(entry);
        _children = null;
    }
    
    public TreeDirectoryEntry GetOrCreateDirectory(string childName) {
        var found = ChildrenDirectories.FirstOrDefault(d => string.Equals(d.DirectoryName, childName));
        if (found != null) return found;
        found = new TreeDirectoryEntry(childName);
        ChildrenDirectories.Add(found);
        _children = null;
        return found;
    }
    
    public ICollection<ITreeItem>? GetChildren() => _children ??= ChildrenDirectories.OrderBy(d => d.GetTreeName()).Cast<ITreeItem>()
        .Concat(ChildrenFiles.OrderBy(f => f.GetTreeName())).ToList();
    
    public IEnumerable<ITreeItem> RecursivelyGrabAllFiles => ChildrenDirectories.SelectMany(d => d.RecursivelyGrabAllFiles).Cast<ITreeItem>().Concat(ChildrenFiles);
}