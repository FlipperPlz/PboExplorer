using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using PboExplorer.Models;

namespace PboExplorer.Utils.Repositories;

public class EntryDataRepository : IDisposable {
    private readonly CacheItemPolicy _entryCachingPolicy = new CacheItemPolicy() {
        RemovedCallback = RemovedCallback
    };

    private static readonly SemaphoreSlim _locker;
    private readonly MemoryCache _repositoryCache;
    private bool _disposed;

    static EntryDataRepository() => _locker = new SemaphoreSlim(1, 2);

    public EntryDataRepository() {
        _repositoryCache = new MemoryCache("EntryRepository");
    }

    public string GetUniqueEntryName(TreeDataEntry entry) =>
        $"{entry.PboDataEntry.EntryParent.GetHashCode()}.{entry.PboDataEntry.GetHashCode()}.{entry.GetHashCode()}";

    public async Task<EntryDataStream> GetOrCreateEntryDataStream(TreeDataEntry key) {
        await _locker.WaitAsync();
        try {
            var uniqueName = GetUniqueEntryName(key);
            if (_repositoryCache.Contains(uniqueName))
                return (EntryDataStream)_repositoryCache.Get(uniqueName);
            var dataStream = new EntryDataStream(key.PboDataEntry);
            _repositoryCache.Add(uniqueName, dataStream, _entryCachingPolicy);
            return dataStream;
        }
        finally {
            _locker.Release();
        }
    }
    
    /// <summary>
    ///  Checks cache for any edited entries
    /// </summary>
    /// <returns>False if all files are stock. </returns>
    public bool AreFilesEdited(out IEnumerable<EntryDataStream> dataStreams) {
        var streams = new List<EntryDataStream>();
        
        foreach (var cache in _repositoryCache) {
            if(cache.Value is not EntryDataStream dataStream) continue;
            if(!dataStream.IsEdited()) continue;
            streams.Add(dataStream);
        }

        dataStreams = streams;

        return streams.Any();
    }
    
    public bool AreFilesEdited() {

        foreach (var cache in _repositoryCache) {
            if (cache.Value is not EntryDataStream dataStream) continue;
            if (!dataStream.IsEdited()) continue;
            return true;
        }

        return false;
    }

    public bool IsCached(TreeDataEntry key) => _repositoryCache.Contains(GetUniqueEntryName(key), string.Empty);

    public bool IsCached(TreeDataEntry key, out EntryDataStream? dataStream) {
        if (!IsCached(key)) {
            dataStream = null;
            return false;
        }

        dataStream = (EntryDataStream)_repositoryCache.Get(GetUniqueEntryName(key));
        return true;
    }

    public bool IsEdited(TreeDataEntry key) {
        if (!IsCached(key, out var entryDataStream) || entryDataStream is null) return false;
        return entryDataStream.IsEdited();
    }

    public bool IsEdited(TreeDataEntry key, out EntryDataStream? dataStream) {
        if (!IsCached(key, out var entryDataStream) || entryDataStream is null) {
            dataStream = null;
            return false;
        }

        dataStream = entryDataStream;
        return entryDataStream.IsEdited();
    }

    public async Task SaveEntryData(TreeDataEntry key) {
        await _locker.WaitAsync();
        try {
            if (IsEdited(key, out var editedStream) && editedStream is not null)
                editedStream.SyncToPBO();
        }
        finally {
            _locker.Release();
        }
    }

    public async Task SaveAllEditedEntries() {
        await _locker.WaitAsync();
        try {
            foreach (var cachedEntry in _repositoryCache) {
                if (cachedEntry.Value is EntryDataStream dataStream && dataStream.IsEdited())
                    dataStream.SyncToPBO();
            }
        }
        finally {
            _locker.Release();
        }
    }

    private static void RemovedCallback(CacheEntryRemovedArguments arg) {
        if (arg.CacheItem.Value is IDisposable disposable) disposable.Dispose();
    }

    public void Dispose() {
        if (_disposed) return;

        _disposed = true;
        _repositoryCache.Dispose();
        GC.SuppressFinalize(this);
    }
}