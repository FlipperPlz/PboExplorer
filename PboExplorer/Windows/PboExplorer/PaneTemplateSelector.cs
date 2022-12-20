using System.Windows.Controls;
using System.Windows;
using PboExplorer.ViewModels;

namespace PboExplorer.Windows.PboExplorer;

class PanesTemplateSelector : DataTemplateSelector
{
    public DataTemplate AboutViewTemplate {get;set;}

    public DataTemplate TextViewTemplate {get;set;}

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item is AboutEntryViewModel)
            return AboutViewTemplate;

        if (item is TextEntryViewModel)
            return TextViewTemplate;

        return base.SelectTemplate(item, container);
    }
}
