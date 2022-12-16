using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PboExplorer.Models;

namespace PboExplorer.Utils.Interfaces; 

public interface ITreeEnumerable: IEntryTreeManaged {
    public ICollection<ITreeItem>? TreeChildren { get; set; }
    public IEnumerable<TreeDataEntry> Files { get; }
    public IEnumerable<TreeDirectoryEntry> Directories { get; }

    public IEnumerable<TreeDataEntry> RecursivelyGrabAllFiles() => 
        Directories.SelectMany(d => d.RecursivelyGrabAllFiles()).Concat(Files);

public T GetOrCreateChild<T>(string title) where T : ITreeItem {
        var folders = title.Split(Path.DirectorySeparatorChar).Where(s => !string.IsNullOrEmpty(s)).ToList();
        if (!folders.Any()) {
            folders.Add("PboExplorer");
            folders.Add($"DisfiguredEntry.{Guid.NewGuid()}");
        }
        switch (typeof(T).Name) {
            case nameof(TreeDirectoryEntry): {
                if (string.IsNullOrWhiteSpace(title)) return (T) (ITreeItem) this;
                var found = Directories.FirstOrDefault(d => string.Equals(d.Title, folders.First()));
                if (found != null) return ((ITreeEnumerable)found).GetOrCreateChild<T>(string.Join(Path.DirectorySeparatorChar, folders.Skip(1)));
                found = new TreeDirectoryEntry(TreeManager) {
                    Title = folders.First(),
                    TreeParent = this,
                };
                AddChild(found);
                var nextPaths = folders.Skip(1).ToList();
                return ((ITreeEnumerable)found).GetOrCreateChild<T>(string.Join(Path.DirectorySeparatorChar, nextPaths));
            }
            case nameof(TreeDataEntry): {
                if (folders.Count == 1) {
                    var foundDataEntry = Files.FirstOrDefault(d => string.Equals(d.Title, folders.First()));
                    if (foundDataEntry is not null) return (T) (ITreeItem) foundDataEntry;
                    
                    var returnFile = new TreeDataEntry(TreeManager) {
                        Title = folders.First(),
                        TreeParent = this,
                    };
                    AddChild(returnFile);
                    return (T) (ITreeItem) returnFile;
                }
                var found = Directories.FirstOrDefault(d => string.Equals(d.Title, folders.First()));
                if (found != null) return ((ITreeEnumerable)found).GetOrCreateChild<T>(string.Join(Path.DirectorySeparatorChar, folders.Skip(1)));
                found = new TreeDirectoryEntry(TreeManager) {
                    Title = folders.First(),
                    TreeParent = this
                };
                AddChild(found);
                var nextPaths = folders.Skip(1).ToList();
                return ((ITreeEnumerable)found).GetOrCreateChild<T>(string.Join(Path.DirectorySeparatorChar, nextPaths));
            }
            default: throw new NotSupportedException();
        }
    }

    public ITreeItem AddChild(ITreeItem child);
    public void RemoveChild(ITreeItem child);
}