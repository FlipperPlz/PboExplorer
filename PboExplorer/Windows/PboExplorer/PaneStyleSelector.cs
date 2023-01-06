using System.Windows.Controls;
using System.Windows;
using PboExplorer.Utils.Interfaces;

namespace PboExplorer.Windows.PboExplorer;

class PanesStyleSelector : StyleSelector
{
    public Style PaneStyle { get; set; }
    public Style DocumentStyle { get; set; }

    public override Style SelectStyle(object item, DependencyObject container)
    {
        return item switch
        {
            IPane => PaneStyle,
            IDocument => DocumentStyle,
            _ => base.SelectStyle(item, container)
        };
    }
}
