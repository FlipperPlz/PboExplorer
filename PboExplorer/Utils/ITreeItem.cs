using System;
using System.IO;
using System.Text;
using System.Windows;
using PboExplorer.Entry;

namespace PboExplorer.Utils;

public interface ITreeItem : IDescribable {

    string IDescribable.Description {
        get => TreePath;
        set => throw new NotSupportedException();
    }
    
    public EntryTreeManager TreeManager { get; set; }

    public string TreePath {
        get {
            var pathBuilder = new StringBuilder();
            if (TreeParent is ITreeItem item) pathBuilder.Append(item.TreePath).Append(Path.DirectorySeparatorChar);
            pathBuilder.Append(Title);
            
            return pathBuilder.ToString();
        }
        set => throw new NotSupportedException();
    }

    public ITreeRoot TreeRoot {
        get;
        set;
    }

    public ITreeEnumerable TreeParent {
        get; 
        set;
    }

}


