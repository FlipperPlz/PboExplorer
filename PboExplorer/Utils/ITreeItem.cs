using System.Collections.Generic;

namespace PboExplorer.Utils; 

public interface ITreeItem {
    string GetTreeName();
    ICollection<ITreeItem>? GetChildren();
}