using AvalonDock.Layout;
using PboExplorer.Utils.Interfaces;
using PboExplorer.ViewModels.Panes;
using System.Linq;

namespace PboExplorer.Windows.PboExplorer;

class LayoutInitializer : ILayoutUpdateStrategy
{
    public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow, ILayoutContainer destinationContainer)
    {
        //AD wants to add the anchorable into destinationContainer
        //just for test provide a new anchorablepane 
        //if the pane is floating let the manager go ahead
        LayoutAnchorablePane? destPane = destinationContainer as LayoutAnchorablePane;
        if (destinationContainer?.FindParent<LayoutFloatingWindow>() != null)
            return false;

        if (anchorableToShow.Content is IPane)
        {
            var leftPane = layout.Descendents()
                .OfType<LayoutAnchorablePane>()
                .FirstOrDefault(d => d.Name == "LeftPane");

            var bottomPane = layout.Descendents()
                .OfType<LayoutAnchorablePane>()
                .FirstOrDefault(d => d.Name == "BottomPane");

            var dest = anchorableToShow.Content switch
            {
                FileTreePaneViewModel or ConfigTreePaneViewModel => leftPane,
                SearchResultsPaneViewModel or EntryInformationPaneViewModel or PboMetadataPaneViewModel => bottomPane,
                _ => bottomPane ?? leftPane
            };

            if (dest != null)
            {
                anchorableToShow.CanClose = false;
                dest.Children.Add(anchorableToShow);
                return true;
            }
        }

        return false;
    }


    public void AfterInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableShown)
    {
    }


    public bool BeforeInsertDocument(LayoutRoot layout, LayoutDocument anchorableToShow, ILayoutContainer destinationContainer)
    {
        return false;
    }

    public void AfterInsertDocument(LayoutRoot layout, LayoutDocument anchorableShown)
    {

    }
}
