using System;
using System.IO;
using System.Windows;
using BisUtils.PBO.Entries;
using PboExplorer.Managers;
using PboExplorer.Utils;
using PboExplorer.Utils.Interfaces;

namespace PboExplorer.TreeItems; 

public class TreeDataEntry : ITreeItem {
    public PboDataEntry PboDataEntry { get; set; }
    public ITreeRoot TreeRoot { get; set; }
    public ITreeEnumerable TreeParent { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    
    public EntryTreeManager TreeManager { get; set; }

    public string FullPath => PboDataEntry.EntryName;
    public string Name => Path.GetFileName(PboDataEntry.EntryName);
    public string Extension => Path.GetExtension(Name);
    public ulong PackedSize => PboDataEntry.PackedSize;
    public ulong OriginalSize => PboDataEntry.OriginalSize;
    public ulong Timestamp => PboDataEntry.TimeStamp;
}