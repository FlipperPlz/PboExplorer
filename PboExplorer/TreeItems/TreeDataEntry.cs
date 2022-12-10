using System.IO;
using System.Text;
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

    public byte[] EntryData {
        get => PboDataEntry.EntryData;
        set => PboDataEntry.EntryData = value;
    }

    public string EntryDataText {
        get => Encoding.UTF8.GetString(EntryData);
        set => EntryData = Encoding.UTF8.GetBytes(value);
    } 

}