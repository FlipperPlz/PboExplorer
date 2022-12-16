using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
            //TODO: This will be hard since we have a cache implemented 
        }
        
        private async void Save(object sender, RoutedEventArgs e) => 
            await TreeManager.DataRepository.SaveAllEditedEntries();

        private void Close(object sender, RoutedEventArgs e) {
            TreeManager.Dispose();
            Close();
        }

        private void CopySelectedEntryName(object sender, RoutedEventArgs e) {
            if(TreeManager.SelectedEntry is null) return;
            Clipboard.SetText(TreeManager.SelectedEntry.FullPath);
        }
        

        private async void CopySelectedEntryData(object sender, RoutedEventArgs e) {
            if(TreeManager.SelectedEntry is null) return;
            Clipboard.SetText(Encoding.UTF8.GetString((await TreeManager.GetCurrentEntryData()).ToArray()));
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

        private async void PromptEntrySave() {
            if (TreeManager.SelectedEntry is null) return;
            var dataStream = await TreeManager.GetCurrentEntryData();
            dataStream.SyncFromStream(new MemoryStream(Encoding.UTF8.GetBytes(TextPreview.Text)));
            if (!dataStream.IsEdited()) return;
            switch(MessageBox.Show("It looks like you've edited this entry, would you like to save it?.\n" +
                                   "Selecting Yes will save the edits of this entry to the corresponding PBO file.\n" +
                                   "Selecting No will save the edits of this entry to cache for later a later sync/edit.\n" +
                                   "Selecting Cancel will revert all changes made.", "PBOExplorer", MessageBoxButton.YesNoCancel)) {
                case MessageBoxResult.Yes:
                    dataStream.SyncToPBO();
                    break;
                case MessageBoxResult.No: break;
                case MessageBoxResult.None:
                case MessageBoxResult.OK:
                case MessageBoxResult.Cancel:
                default:
                    dataStream.SyncFromPbo();
                    break;
            }
        }


        private async void ShowPboEntry(object sender, RoutedPropertyChangedEventArgs<object> e) {
            switch (e.NewValue) {
                case TreeDataEntry treeDataEntry: {
                    PromptEntrySave();
                    TreeManager.SelectedEntry = treeDataEntry;
                    TextPreview.Visibility = Visibility.Visible;
                    TextPreview.Text = Encoding.UTF8.GetString((await TreeManager.DataRepository.GetOrCreateEntryDataStream(treeDataEntry)).ToArray());
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

        private void CanSave(object sender, CanExecuteRoutedEventArgs e) =>
            e.CanExecute = TreeManager.DataRepository.AreFilesEdited();
    }
}
