using System.IO;
using BisUtils.PBO.Entries;
using PboExplorer.Utils;

namespace PboExplorer.TreeItems; 

public class TreeDataEntry : ITreeItem {
    public PboDataEntry PboDataEntry { get; set; }
    public ITreeRoot TreeRoot { get; set; }
    public ITreeEnumerable TreeParent { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }


    public string FullPath => PboDataEntry.EntryName;
    public string Name => Path.GetFileName(PboDataEntry.EntryName);
    public string Extension => Path.GetExtension(Name);
    public ulong PackedSize => PboDataEntry.PackedSize;
    public ulong OriginalSize => PboDataEntry.OriginalSize;
    public ulong Timestamp => PboDataEntry.TimeStamp;
}