using System;
using System.IO;
using System.Text;
using System.Windows;

namespace PboExplorer.Utils; 

public interface ITreeEnumerableItem : ITreeItem, ITreeEnumerable {
    string ITreeItem.TreePath {
        get {
            var pathBuilder = new StringBuilder();
            if (TreeParent is ITreeItem item) pathBuilder.Append(item.TreePath).Append(Path.DirectorySeparatorChar);
            pathBuilder.Append(Title).Append(Path.DirectorySeparatorChar);
            
            return pathBuilder.ToString();
        }
        set => throw new NotSupportedException();
    }
}