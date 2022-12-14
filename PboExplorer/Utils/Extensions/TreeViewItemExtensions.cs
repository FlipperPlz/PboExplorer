using System.Windows.Controls;

namespace PboExplorer.Utils.Extensions; 

public static class TreeViewItemExtensions {
    public static TreeViewItem ContainerFromItemRecursive(this ItemContainerGenerator root, object item)
    {
        var treeViewItem = root.ContainerFromItem(item) as TreeViewItem;
        if (treeViewItem != null)
            return treeViewItem;
        foreach (var subItem in root.Items)
        {
            treeViewItem = root.ContainerFromItem(subItem) as TreeViewItem;
            var search = treeViewItem?.ItemContainerGenerator.ContainerFromItemRecursive(item);
            if (search != null)
                return search;
        }
        return null;
    }
}