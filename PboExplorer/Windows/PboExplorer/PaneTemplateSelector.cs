using System.Windows.Controls;
using System.Windows;
using PboExplorer.ViewModels;
using PboExplorer.ViewModels.Panes;

namespace PboExplorer.Windows.PboExplorer;

// TODO: Refactor
class PanesTemplateSelector : DataTemplateSelector
{
    public DataTemplate AboutViewTemplate { get; set; }

    public DataTemplate TextViewTemplate { get; set; }

    public DataTemplate FileTreePaneTemplate { get; set; }
    public DataTemplate ConfigTreePaneTemplate { get; set; }
    public DataTemplate SearchResultsPaneTemplate { get; set; }
    public DataTemplate PboMetaDataPaneTemplate { get; set; }
    public DataTemplate EntryInformationPaneTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        return item switch
        {
            AboutEntryViewModel => AboutViewTemplate,
            TextEntryViewModel => TextViewTemplate,
            FileTreePaneViewModel => FileTreePaneTemplate,
            ConfigTreePaneViewModel => ConfigTreePaneTemplate,
            SearchResultsPaneViewModel => SearchResultsPaneTemplate,
            PboMetadataPaneViewModel => PboMetaDataPaneTemplate,
            EntryInformationPaneViewModel => EntryInformationPaneTemplate,
            _ => base.SelectTemplate(item, container)
        };
    }
}
