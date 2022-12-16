using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    public ObservableCollection<FileSearchResult> SearchResults;

    private bool _disposed;

    public EntryTreeManager(PboFile pboFile) {
        PboFile = pboFile;
        EntryRoot = new EntryTreeRoot(this);
        SearchResults = new ObservableCollection<FileSearchResult>();
        SyncEntryRoot();
    }

    public void ClearSearchResults() => SearchResults.Clear();

    private void SyncEntryRoot() {
        foreach (var entry in PboFile.GetDataEntries())
            ((ITreeEnumerable)EntryRoot).GetOrCreateChild<TreeDataEntry>(entry.EntryName).PboDataEntry = entry;
    }
    
    public async Task<IEnumerable<FileSearchResult>> SearchForString(string search, bool cacheIfNotAlready) {
        var results = new List<FileSearchResult>();
        foreach (var dataEntry in EntryRoot.RecursivelyGrabAllFiles())
        {
            var searchResult = await dataEntry.SearchForString(search, cacheIfNotAlready);
            if (!searchResult.SearchResults.Any()) continue;
            results.Add(searchResult);
        }
            
        return results;
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