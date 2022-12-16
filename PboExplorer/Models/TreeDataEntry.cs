using System.IO;
using BisUtils.PBO.Entries;
using PboExplorer.Utils.Interfaces;
using PboExplorer.Utils.Managers;
using PboExplorer.Utils.Repositories;

namespace PboExplorer.Models; 

public class TreeDataEntry : ITreeItem {
    public PboDataEntry PboDataEntry { get; set; }
    public ITreeRoot TreeRoot { get; set; }
    public ITreeEnumerable TreeParent { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    
    public EntryTreeManager TreeManager { get; set; }
    public EntryDataRepository DataRepository { get; set; }

    public TreeDataEntry(EntryTreeManager treeManager) {
        TreeManager = treeManager;
        DataRepository = treeManager.DataRepository;
        TreeRoot = treeManager.EntryRoot;
    }

    public string FullPath => PboDataEntry.EntryName;
    public string Name => Path.GetFileName(PboDataEntry.EntryName);
    public string Extension => Path.GetExtension(Name);
    public ulong PackedSize => PboDataEntry.PackedSize;
    public ulong OriginalSize => PboDataEntry.OriginalSize;
    public ulong Timestamp => PboDataEntry.TimeStamp;
}