using System;
using System.Threading.Tasks;
using BisUtils.PBO;
using PboExplorer.Models;
using PboExplorer.Utils.Interfaces;
using PboExplorer.Utils.Repositories;

namespace PboExplorer.Utils.Managers;

public class EntryTreeManager : IDisposable {
    public readonly PboFile PboFile;
    public readonly EntryTreeRoot EntryRoot;
    public readonly EntryDataRepository DataRepository = new();
    public TreeDataEntry? SelectedEntry = null;

    private bool _disposed;

    public EntryTreeManager(PboFile pboFile) {
        PboFile = pboFile;
        EntryRoot = new EntryTreeRoot(this);
        SyncEntryRoot();
    }

    private void SyncEntryRoot() {
        foreach (var entry in PboFile.GetDataEntries())
            ((ITreeEnumerable)EntryRoot).GetOrCreateChild<TreeDataEntry>(entry.EntryName).PboDataEntry = entry;
    }

    public async Task<EntryDataStream> GetCurrentEntryData() {
        if (SelectedEntry is null) throw new Exception();
        return await DataRepository.GetOrCreateEntryDataStream(SelectedEntry);
    }


    public void Dispose() {
        if (_disposed) return;

        _disposed = true;
        DataRepository.Dispose();
        PboFile.Dispose();
        GC.SuppressFinalize(this);
    }
}