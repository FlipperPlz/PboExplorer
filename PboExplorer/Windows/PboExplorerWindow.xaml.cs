using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using BisUtils.PBO;
using BisUtils.PBO.Entries;
using Microsoft.Win32;
using PboExplorer.TreeItems;
using PboExplorer.Utils;
using Path = System.IO.Path;

namespace PboExplorer.Windows
{

    internal struct EntryInformation {
        public string Key { get; set; }
        public string Value { get; set; }
    }
    /// <summary>
    /// Interaction logic for PboExplorerWindow.xaml
    /// </summary>
    public partial class PboExplorerWindow : Window {
        private readonly PboFile _pboFile;
        private readonly ObservableCollection<ITreeItem> EntryList = new();
        
        private readonly ObservableCollection<EntryInformation> SelectedEntryInfo = new();

        private TreeDirectoryEntry? SelectedDirectory { get; set; }
        private TreeDataEntry? SelectedEntry { get; set; }

        public PboExplorerWindow(PboFile pboFile) {
            InitializeComponent();

            _pboFile = pboFile;
            PboView.ItemsSource = EntryList;

            foreach (var entry in pboFile.GetPboEntries().Where(e => e is PboDataEntry)) {
                var parent = (Path.GetDirectoryName(entry.EntryName) ?? string.Empty).Trim('/','\\');
                if (string.IsNullOrEmpty(parent)) EntryList.Add(new TreeDataEntry((PboDataEntry) entry));
                else {
                    var entryDirectory =  GetOrCreateDirectory(parent);
                    entryDirectory.AddEntry(new TreeDataEntry((PboDataEntry) entry, entryDirectory));
                }
            }
        }

        public TreeDirectoryEntry GetOrCreateDirectory(string directoryName) {
            var folders = directoryName.Split(Path.DirectorySeparatorChar).Where(s => !string.IsNullOrEmpty(s));
            var found =
                EntryList.Where(e => e is TreeDirectoryEntry).Cast<TreeDirectoryEntry>()
                    .FirstOrDefault(d => string.Equals(d.TreeTitle, folders.First()));
            if (found is not null) return found.GetOrCreateDirectory(string.Join(Path.DirectorySeparatorChar, folders.Skip(1)));;
            found = new TreeDirectoryEntry(folders.First());
            EntryList.Add(found);
            
            
            var nextPaths = folders.Skip(1).ToList();
        
            return nextPaths.Count != 0 ? found.GetOrCreateDirectory(string.Join(Path.DirectorySeparatorChar, nextPaths)) : found;
        }
        
        private void ResetView() {
            SearchBox.Clear();
            SelectedEntry = null;
            SelectedDirectory = null;
            TextPreview.Text = string.Empty;
            SearchResultsView.ItemsSource = null;
            EntryInformationGrid.ItemsSource = null;
            TreeViewCtxDelete.Visibility = Visibility.Hidden;
            TreeViewCtxCopyData.Visibility = Visibility.Hidden;
            TreeViewCtxCopyName.Visibility = Visibility.Hidden;
            SelectedEntryInfo.Clear();
        }
        
        private void SaveAs(object sender, RoutedEventArgs e) {
            var dialog = new SaveFileDialog() {
                Title = "Save PBO File",
                Filter = "PBO File|*.pbo",
                DefaultExt = "pbo"
            };

            if (dialog.ShowDialog() is true) {
                using var fileStream = File.Create(dialog.FileName);
                using var writer = new BinaryWriter(fileStream);
                _pboFile.WriteBinary(writer, PboBinarizationOptions.DebugOptions);
                writer.Flush();
            }
        }

        private void Close(object sender, RoutedEventArgs e) {
            _pboFile.PboStream.Close();
            Close();
        }

        private void CopySelectedEntryName(object sender, RoutedEventArgs e) {
            if(SelectedEntry is null) return;
            Clipboard.SetText(SelectedEntry.FullPath);
        }

        private void CopySelectedEntryData(object sender, RoutedEventArgs e) {
            if(SelectedEntry is null) return;
            Clipboard.SetText(Encoding.UTF8.GetString(SelectedEntry.GetEntryData.ToArray()));
        }

        private void DeleteSelectedEntry(object sender, RoutedEventArgs e) {
            SelectedEntry?.DeleteEntry();
        }

        private void AddEntryWizard(object sender, RoutedEventArgs e) {
            
        }

        private void PboEntry_Drop(object sender, DragEventArgs e) {
            
        }

        private void ShowPboEntry(object sender, RoutedPropertyChangedEventArgs<object> e) {
            TextPreview.Text = string.Empty;
            SelectedEntry = null;
            SelectedDirectory = null;
            TreeViewCtxDelete.Visibility = Visibility.Hidden;
            TreeViewCtxCopyData.Visibility = Visibility.Hidden;
            TreeViewCtxCopyName.Visibility = Visibility.Hidden;
            SelectedEntryInfo.Clear();
            EntryInformationGrid.ItemsSource = SelectedEntryInfo;


            switch (e.NewValue) {
                case TreeDataEntry dataEntry: {
                    SelectedEntry = dataEntry;
                    TextPreview.Text = Encoding.UTF8.GetString(SelectedEntry.GetEntryData);
                    TreeViewCtxDelete.Visibility = Visibility.Visible;
                    TreeViewCtxCopyData.Visibility = Visibility.Visible;
                    TreeViewCtxCopyName.Visibility = Visibility.Visible;
                    SelectedEntryInfo.Add(new EntryInformation() { Key = "Entry Name", Value = SelectedEntry.Name });
                    SelectedEntryInfo.Add(new EntryInformation() { Key = "Full Path", Value = SelectedEntry.FullPath });
                    SelectedEntryInfo.Add(new EntryInformation() { Key = "Tree Path", Value = SelectedEntry.TreePath });
                    SelectedEntryInfo.Add(new EntryInformation() { Key = "Entry Size", Value = SelectedEntry.OriginalSize.ToString() });
                    SelectedEntryInfo.Add(new EntryInformation() { Key = "Stored Size", Value = SelectedEntry.PackedSize.ToString() });
                    SelectedEntryInfo.Add(new EntryInformation() { Key = "Time Stamp", Value = SelectedEntry.Timestamp.ToString() });
                    SelectedEntryInfo.Add(new EntryInformation() { Key = "Written Offset", Value = string.Empty });
                    SelectedEntryInfo.Add(new EntryInformation() { Key = "Calculated Offset", Value = (_pboFile.DataBlockStartOffset + SelectedEntry.PboDataEntry.EntryDataStartOffset).ToString() });

                    break;
                }
                case TreeDirectoryEntry directoryEntry: {
                    SelectedDirectory = directoryEntry;
                    SelectedEntryInfo.Add(new EntryInformation() { Key = "Directory Name", Value = SelectedDirectory.TreeTitle });
                    SelectedEntryInfo.Add(new EntryInformation() { Key = "Tree Path", Value = SelectedDirectory.TreePath });

                    break;
                }
                default: return;
            }
        }

        private void ConfigEntry_Drop(object sender, DragEventArgs e) {
            
        }

        private void ShowConfigEntry(object sender, RoutedPropertyChangedEventArgs<object> e) {
        }

        private void ShowSearchResult(object sender, SelectionChangedEventArgs e) {
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) {
            
        }

        private void SubmitSearch(object sender, RoutedEventArgs e) {
            
        }
    }
}
