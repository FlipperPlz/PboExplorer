using System.Collections.Generic;
using PboExplorer.TreeItems;

namespace PboExplorer.Utils; 

public interface ITreeEnumerable {
    public ICollection<ITreeItem>? TreeChildren { get; set; }
    public IEnumerable<TreeDataEntry> RecursivelyGrabAllFiles();
    public T GetOrCreateChild<T>(string title) where T : ITreeItem;
    public ITreeItem AddChild(ITreeItem child);
    public void RemoveChild(ITreeItem child);
}