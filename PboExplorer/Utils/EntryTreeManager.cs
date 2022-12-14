using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using BisUtils.PBO;
using BisUtils.PBO.Entries;
using PboExplorer.TreeItems;

namespace PboExplorer.Entry;

public class EntryTreeManager : IDisposable {
    public readonly PboFile PboFile;
    public readonly EntryTreeRoot EntryRoot = new();
    public readonly EntryDataRepository DataRepository;
    public TreeDataEntry? SelectedEntry;

    private bool _disposed;

    public EntryTreeManager(PboFile pboFile) {
        PboFile = pboFile;
        DataRepository = new EntryDataRepository();
        SelectedEntry = null;
        SyncEntryRoot();
    }

    private void SyncEntryRoot() {
        foreach (var entry in PboFile.GetDataEntries())
            EntryRoot.GetOrCreateChild<TreeDataEntry>(entry.EntryName).PboDataEntry = entry;
    }


    public void Dispose() {
        if (_disposed) return;

        _disposed = true;
        DataRepository.Dispose();
        PboFile.Dispose();
        GC.SuppressFinalize(this);
    }
    
    public class EntryDataStream : MemoryStream {
        public readonly PboDataEntry PboDataEntry;
        public readonly byte[] OriginalDataCRC;

        public EntryDataStream(PboDataEntry pboDataEntry) : base(pboDataEntry.EntryData) {
            PboDataEntry = pboDataEntry;
            OriginalDataCRC = CalculateChecksum();
        }

        public bool IsEdited() => CalculateChecksum() != OriginalDataCRC;
    
        public byte[] CalculateChecksum() {
        #pragma warning disable SYSLIB0021
            using var sha1 = new System.Security.Cryptography.SHA1CryptoServiceProvider();
        #pragma warning restore SYSLIB0021
            return sha1.ComputeHash(ToArray());
        }

        public void SyncToPBO() => PboDataEntry.EntryData = ToArray();
    }

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

        public async Task<EntryDataStream> GetEntryDataStream(TreeDataEntry key) {
            await _locker.WaitAsync();
            try {
                var uniqueName = GetUniqueEntryName(key);
                if (_repositoryCache.Contains(uniqueName, "entries"))
                    return (EntryDataStream)_repositoryCache.Get(uniqueName, "entries");
                var dataStream = new EntryDataStream(key.PboDataEntry);
                _repositoryCache.Add(uniqueName, dataStream, _entryCachingPolicy, "entries");
                return dataStream;
            }
            finally {
                _locker.Release();
            }
        }

        public bool IsCached(TreeDataEntry key) => _repositoryCache.Contains(GetUniqueEntryName(key), "entries");

        public bool IsCached(TreeDataEntry key, out EntryDataStream? dataStream) {
            if (!IsCached(key)) {
                dataStream = null;
                return false;
            }

            dataStream = (EntryDataStream)_repositoryCache.Get(GetUniqueEntryName(key), "entries");
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

        public IEnumerable<EntryDataStream> GetCachedEntries() =>
            _repositoryCache.GetValues("entries").Values.Cast<EntryDataStream>();

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
                foreach (var cachedEntry in GetCachedEntries()) {
                    if (cachedEntry.IsEdited()) cachedEntry.SyncToPBO();
                }
            }
            finally {
                _locker.Release();
            }
        }

        private static void RemovedCallback(CacheEntryRemovedArguments arg) {
            if (!(arg.RemovedReason == CacheEntryRemovedReason.Removed ||
                  arg.CacheItem.Value is not IDisposable disposable))
                disposable.Dispose();
        }

        public void Dispose() {
            if (_disposed) return;

            _disposed = true;
            _repositoryCache.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}