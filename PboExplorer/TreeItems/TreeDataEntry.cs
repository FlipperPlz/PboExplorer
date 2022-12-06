using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BisUtils.PBO;
using BisUtils.PBO.Entries;
using PboExplorer.Utils;

namespace PboExplorer.TreeItems; 

public class TreeDataEntry : ITreeItem {
    public readonly PboDataEntry PboDataEntry;
    private IPboFile _parentPbo => PboDataEntry.EntryParent;
    public ITreeItem? TreeParent { get; set; }
    public string TreeTitle {
        get => Name;
        set {
            var place = TreePath.LastIndexOf(Name, StringComparison.Ordinal);
            _parentPbo.RenameEntry(PboDataEntry, TreePath.Remove(place, Name.Length).Insert(place, value));
        }
    }

    public string TreePath {
        get {
            var pathBuilder = new StringBuilder();
            if (TreeParent is not null) pathBuilder.Append(TreeParent.TreePath).Append(Path.DirectorySeparatorChar);
            pathBuilder.Append(TreeTitle);
            
            return pathBuilder.ToString();
        }
        set => _parentPbo.RenameEntry(PboDataEntry, value, false);
    }

    public ICollection<ITreeItem>? TreeChildren {
        get => null;
        set => throw new NotSupportedException();
    }

    public TreeDataEntry(PboDataEntry dataEntry, ITreeItem? parent = null) {
        PboDataEntry = dataEntry;
        TreeParent = parent;
    }

    public void DeleteEntry() {
        if (TreeParent is not null) TreeParent.TreeChildren!.Remove(this);
        _parentPbo.DeleteEntry(PboDataEntry, false);
    }
    
    public string FullPath => PboDataEntry.EntryName;
    public string Name => Path.GetFileName(PboDataEntry.EntryName);
    public string Extension => Path.GetExtension(Name);
    public ulong PackedSize => PboDataEntry.PackedSize;
    public ulong OriginalSize => PboDataEntry.OriginalSize;
    public ulong Timestamp => PboDataEntry.TimeStamp;
    public byte[] GetEntryData => PboDataEntry.EntryData;
}