using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using BisUtils.PBO;
using PboExplorer.Models;
using PboExplorer.Utils.Extensions;
using PboExplorer.Utils.Managers;

namespace PboExplorer.Windows.PboExplorer
{
    /// <summary>
    /// Interaction logic for PboExplorerWindow.xaml
    /// </summary>
    public partial class PboExplorerWindow {
        private readonly EntryTreeManager TreeManager;
        
        public PboExplorerWindow(PboFile pboFile) {
            InitializeComponent();
            TreeManager = new EntryTreeManager(pboFile);
            PboView.ItemsSource = TreeManager.EntryRoot.TreeChildren;
        }

        private void SaveAs(object sender, RoutedEventArgs e) {
            throw new NotImplementedException();
        }
        
        private async void Save(object sender, RoutedEventArgs e) {
            await TreeManager.DataRepository.SaveAllEditedEntries();
        }

        private void Close(object sender, RoutedEventArgs e) {
            TreeManager.Dispose();
            Close();
        }

        private void CopySelectedEntryName(object sender, RoutedEventArgs e) {
            if(TreeManager.SelectedEntry is null) return;
            Clipboard.SetText(TreeManager.SelectedEntry.FullPath);
        }
        

        private void CopySelectedEntryData(object sender, RoutedEventArgs e) {
            if(TreeManager.SelectedEntry is null) return;
            throw new NotImplementedException();
        }

        private void DeleteSelectedEntry(object sender, RoutedEventArgs e) {
            throw new NotImplementedException();
        }

        private void AddEntryWizard(object sender, RoutedEventArgs e) {
            throw new NotImplementedException();
        }

        private void PboEntry_Drop(object sender, DragEventArgs e) {
            throw new NotImplementedException();
        }

        private async void SaveSelectedEntryToCache(TreeViewItem eOriginalSource) {
            if (TreeManager.SelectedEntry is null) return;
            var dataStream = await TreeManager.DataRepository.GetEntryDataStream(TreeManager.SelectedEntry);
            dataStream.SetLength(0);
            dataStream.Write(Encoding.UTF8.GetBytes(TextPreview.Text));
            if (dataStream.IsEdited()) {
                //TODO: Mark Entry As Edited In Tree
            }
        }


        private async void ShowPboEntry(object sender, RoutedPropertyChangedEventArgs<object> e) {
            switch (e.NewValue) {
                case TreeDataEntry treeDataEntry: {
                    SaveSelectedEntryToCache(PboView.ItemContainerGenerator.ContainerFromItemRecursive(PboView.SelectedItem));
                    TreeManager.SelectedEntry = treeDataEntry;
                    TextPreview.Visibility = Visibility.Visible;
                    TextPreview.Text = Encoding.UTF8.GetString((await TreeManager.DataRepository.GetEntryDataStream(treeDataEntry)).ToArray());
                    break;    
                }
                default: return;
            }
        }
        
        

        private void ConfigEntry_Drop(object sender, DragEventArgs e) {
            throw new NotImplementedException();
        }

        private void ShowConfigEntry(object sender, RoutedPropertyChangedEventArgs<object> e) {
            throw new NotImplementedException();
        }

        private void ShowSearchResult(object sender, SelectionChangedEventArgs e) {
            throw new NotImplementedException();
        }

        private void SubmitSearch(object sender, RoutedEventArgs e) {
            throw new NotImplementedException();
        }

        private async void PreviewEdited(object sender, TextChangedEventArgs e) {
           
        }
        
        private static T? GetFrameworkElementByName<T>(DependencyObject parent, string name) where T : FrameworkElement {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++) {

                var child = VisualTreeHelper.GetChild(parent, i) as FrameworkElement;

                if (child is null || child.Name != name) continue;
                var found = child as T;
                if (found is not null) return found;
            }

            return null;
        }

    }
}
