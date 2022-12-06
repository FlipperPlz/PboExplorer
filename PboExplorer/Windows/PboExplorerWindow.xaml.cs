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
    /// <summary>
    /// Interaction logic for PboExplorerWindow.xaml
    /// </summary>
    public partial class PboExplorerWindow : Window {
        private readonly PboFile _pboFile;
        private readonly ObservableCollection<ITreeItem> EntryList = new();

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
            var folders = directoryName.Split(Path.DirectorySeparatorChar);
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
            TextPreview.Text = string.Empty;
            SearchResultsView.ItemsSource = null;
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

            switch (e.NewValue) {
                case TreeDataEntry dataEntry: {
                    SelectedEntry = dataEntry;
                    TextPreview.Text = Encoding.UTF8.GetString(SelectedEntry.GetEntryData);
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
