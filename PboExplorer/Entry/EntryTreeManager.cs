using System;
using System.Threading.Tasks;
using BisUtils.PBO;
using BisUtils.PBO.Entries;
using BisUtils.PBO.Interfaces;
using PboExplorer.TreeItems;

namespace PboExplorer.Entry;

public class EntryTreeManager : IDisposable {
    public readonly PboFile PboFile;
    public readonly EntryTreeRoot EntryRoot = new();
    public readonly EntryDataRepository EntryDataRepository;
    public TreeDataEntry? SelectedEntry;

    private bool _disposed;


    public EntryTreeManager(PboFile pboFile) {
        PboFile = pboFile;
        EntryDataRepository = new EntryDataRepository();
        SyncEntryRoot();
    }

    private void SyncEntryRoot() {
        foreach (var entry in PboFile.GetDataEntries()) {
            EntryRoot.GetOrCreateChild<TreeDataEntry>(entry.EntryName).PboDataEntry = entry;
        }
    }
    

    public void Dispose() {
        if(_disposed) return;

        _disposed = true;
        EntryDataRepository.Dispose();
        PboFile.Dispose();
        GC.SuppressFinalize(this);
    }
}