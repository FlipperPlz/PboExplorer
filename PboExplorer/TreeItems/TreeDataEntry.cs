using System.Collections.Generic;
using System.IO;
using System.Windows.Shapes;
using BisUtils.PBO;
using BisUtils.PBO.Entries;
using PboExplorer.Utils;
using Path = System.IO.Path;

namespace PboExplorer.TreeItems; 

public class TreeDataEntry : ITreeItem {
    private readonly PboDataEntry _pboDataEntry;
    private readonly IPboFile _parentPbo;
    
    public string FullPath => _pboDataEntry.EntryName;
    public string Name => Path.GetFileName(_pboDataEntry.EntryName);
    public string Extension => Path.GetExtension(Name);
    public ulong PackedSize => _pboDataEntry.PackedSize;
    public ulong OriginalSize => _pboDataEntry.OriginalSize;
    public ulong Timestamp => _pboDataEntry.TimeStamp;

    public TreeDataEntry(PboDataEntry pboDataEntry) {
        _pboDataEntry = pboDataEntry;
        _parentPbo = pboDataEntry.EntryParent;
    }

    public void DeleteEntry() {
        _pboDataEntry.EntryParent.DeleteEntry(_pboDataEntry, false);
    }

    public string GetTreeName() => Name;

    public MemoryStream GetDataStream() => new(_pboDataEntry.EntryData);
    
    public ICollection<ITreeItem>? GetChildren() => null;
}