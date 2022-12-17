using System.Windows.Controls;
using System.Windows;
using PboExplorer.Models;

namespace PboExplorer.Windows.PboExplorer;

class PanesTemplateSelector : DataTemplateSelector
{
    public DataTemplate AboutViewTemplate {get;set;}

    public DataTemplate TextViewTemplate {get;set;}

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item is AboutEntry)
            return AboutViewTemplate;

        if (item is TextEntry)
            return TextViewTemplate;

        return base.SelectTemplate(item, container);
    }
}
