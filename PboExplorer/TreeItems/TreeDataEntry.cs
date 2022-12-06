using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BisUtils.PBO;
using BisUtils.PBO.Entries;
using PboExplorer.Utils;

namespace PboExplorer.TreeItems; 

public class TreeDataEntry : ITreeItem {
    private readonly PboDataEntry _pboDataEntry;
    private IPboFile _parentPbo => _pboDataEntry.EntryParent;
    public ITreeItem? TreeParent { get; set; }
    public string TreeTitle {
        get => Name;
        set {
            var place = TreePath.LastIndexOf(Name, StringComparison.Ordinal);
            _parentPbo.RenameEntry(_pboDataEntry, TreePath.Remove(place, Name.Length).Insert(place, value));
        }
    }

    public string TreePath {
        get {
            var pathBuilder = new StringBuilder();
            if (TreeParent is not null) pathBuilder.Append(TreeParent.TreePath).Append(Path.DirectorySeparatorChar);
            pathBuilder.Append(TreeTitle);
            
            return pathBuilder.ToString();
        }
        set => _parentPbo.RenameEntry(_pboDataEntry, value, false);
    }

    public ICollection<ITreeItem>? TreeChildren {
        get => null;
        set => throw new NotSupportedException();
    }

    public TreeDataEntry(PboDataEntry dataEntry, ITreeItem? parent = null) {
        _pboDataEntry = dataEntry;
        TreeParent = parent;
    }

    public void DeleteEntry() {
        if (TreeParent is not null) TreeParent.TreeChildren!.Remove(this);
        _parentPbo.DeleteEntry(_pboDataEntry, false);
    }
    
    public string FullPath => _pboDataEntry.EntryName;
    public string Name => Path.GetFileName(_pboDataEntry.EntryName);
    public string Extension => Path.GetExtension(Name);
    public ulong PackedSize => _pboDataEntry.PackedSize;
    public ulong OriginalSize => _pboDataEntry.OriginalSize;
    public ulong Timestamp => _pboDataEntry.TimeStamp;
    public byte[] GetEntryData => _pboDataEntry.EntryData;
}