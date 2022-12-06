using System.Collections.Generic;

namespace PboExplorer.Utils; 

public interface ITreeItem {
    public string TreeTitle { get; set; }
    public string TreePath { get; set; }
    public ICollection<ITreeItem>? TreeChildren { get; set; }
    public ITreeItem? TreeParent { get; set; }
}