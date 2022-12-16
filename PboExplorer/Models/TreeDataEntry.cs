using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BisUtils.PBO.Entries;
using PboExplorer.Utils.Extensions;
using PboExplorer.Utils.Interfaces;
using PboExplorer.Utils.Managers;
using PboExplorer.Utils.Repositories;

namespace PboExplorer.Models; 

public class TreeDataEntry : ITreeItem, IComparable<TreeDataEntry> {
    private readonly ObservableCollection<ITreeItem> _entryList = new();

    
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

    public async Task<FileSearchResult> SearchForString(string search, bool cacheIfNotAlready) {
        var searchResults = new List<SearchResult>();
        MemoryStream? stream = null;
        var dispose = false;
        if (cacheIfNotAlready || DataRepository.IsCached(this))
            stream = await DataRepository.GetOrCreateEntryDataStream(this);
        if (stream is null) {
            stream = new MemoryStream(PboDataEntry.EntryData);
            dispose = true;
        }

        using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: !dispose);
        var line = await reader.ReadLineAsync();
        var lineNumber = 1;
        while (line is not null) {
            var indexOfSearch = line.IndexOf(search, StringComparison.Ordinal);
            if (indexOfSearch > -1) searchResults.Add(new SearchResult( lineNumber, indexOfSearch, (string.Join(string.Empty, line.Skip(indexOfSearch))).Truncate(25), this));
            lineNumber++;
            line = await reader.ReadLineAsync();
        }

        return new FileSearchResult(this, searchResults);
    }


    public int CompareTo(TreeDataEntry? other) {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        var titleComparison = string.Compare(Title, other.Title, StringComparison.Ordinal);
        return titleComparison != 0 ? titleComparison : string.Compare(Description, other.Description, StringComparison.Ordinal);
    }

    public int CompareTo(ITreeItem? other) {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return other switch {
            TreeDirectoryEntry => 1,
            TreeDataEntry treeDataEntry => CompareTo(treeDataEntry),
            _ => 0
        };
    }
}