using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using PboExplorer.Utils.Interfaces;

namespace PboExplorer.Windows.PboExplorer;

class PanesStyleSelector : StyleSelector
{
    public Style DocumentStyle { get; set; }

    public override System.Windows.Style SelectStyle(object item, System.Windows.DependencyObject container)
    {
        if (item is IDocument)
            return DocumentStyle;

        return base.SelectStyle(item, container);
    }
}
