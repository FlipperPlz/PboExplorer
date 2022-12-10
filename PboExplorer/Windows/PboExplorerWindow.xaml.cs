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

namespace PboExplorer.Windows
{

    internal struct EntryInformation {
        public string Key { get; set; }
        public string Value { get; set; }
    }
    /// <summary>
    /// Interaction logic for PboExplorerWindow.xaml
    /// </summary>
    public partial class PboExplorerWindow {
        private readonly PboFile _pboFile;
        private readonly EntryTreeRoot EntryRoot = new();
        private readonly ObservableCollection<TreeDataEntry> OpenDataEntries = new();

        private TreeDirectoryEntry? SelectedDirectory { get; set; }
        private TreeDataEntry? SelectedEntry { get; set; }

        public PboExplorerWindow(PboFile pboFile) {
            InitializeComponent();

            _pboFile = pboFile;
            PboView.ItemsSource = EntryRoot.TreeChildren;
            PreviewTabControl.ItemsSource = OpenDataEntries;

            foreach (var entry in pboFile.GetPboEntries().Where(e => e is PboDataEntry)) {
                EntryRoot.GetOrCreateChild<TreeDataEntry>(entry.EntryName).PboDataEntry = (PboDataEntry) entry;
            }
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
            Clipboard.SetText(Encoding.UTF8.GetString(SelectedEntry.EntryData));
        }

        private void DeleteSelectedEntry(object sender, RoutedEventArgs e) {
            
        }

        private void AddEntryWizard(object sender, RoutedEventArgs e) {
            
        }

        private void PboEntry_Drop(object sender, DragEventArgs e) {
            
        }

        private void ShowPboEntry(object sender, RoutedPropertyChangedEventArgs<object> e) {
            SelectedEntry = null;
            SelectedDirectory = null;
            TreeViewCtxDelete.Visibility = Visibility.Hidden;
            TreeViewCtxCopyData.Visibility = Visibility.Hidden;
            TreeViewCtxCopyName.Visibility = Visibility.Hidden;
            

            switch (e.NewValue) {
                case TreeDataEntry dataEntry: {
                    SelectedEntry = dataEntry;
                    if(!OpenDataEntries.Contains(dataEntry)) OpenDataEntries.Add(dataEntry);
                    PreviewTabControl.SelectedItem = dataEntry;
                    TreeViewCtxDelete.Visibility = Visibility.Visible;
                    TreeViewCtxCopyData.Visibility = Visibility.Visible;
                    TreeViewCtxCopyName.Visibility = Visibility.Visible;
                    EntryInformationGrid.ItemsSource = new ObservableCollection<EntryInformation>() {
                        new EntryInformation { Key = "Entry Name", Value = SelectedEntry.Name },
                        new EntryInformation { Key = "Full Path", Value = SelectedEntry.FullPath },
                        new EntryInformation { Key = "Tree Path", Value = SelectedEntry.Description },
                        new EntryInformation { Key = "Entry Size", Value = SelectedEntry.OriginalSize.ToString() },
                        new EntryInformation { Key = "Stored Size", Value = SelectedEntry.PackedSize.ToString() },
                        new EntryInformation { Key = "Time Stamp", Value = SelectedEntry.Timestamp.ToString() },
                        new EntryInformation { Key = "Written Offset", Value = string.Empty },
                        new EntryInformation {
                            Key = "Calculated Offset",
                            Value = (_pboFile.DataBlockStartOffset + SelectedEntry.PboDataEntry.EntryDataStartOffset)
                                .ToString()
                        }
                    };

                    break;
                }
                case TreeDirectoryEntry directoryEntry: {
                    SelectedDirectory = directoryEntry;
                    EntryInformationGrid.ItemsSource = new ObservableCollection<EntryInformation>() {
                        new EntryInformation() { Key = "Directory Name", Value = SelectedDirectory.Title },
                        new EntryInformation() { Key = "Tree Path", Value = SelectedDirectory.Description }
                    };
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
        
        private void ClosePreview(object sender, RoutedEventArgs e) {
            Console.WriteLine();
            throw new System.NotImplementedException();
        }

        private void PreviewTabControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
            if ((((TabControl)e.Source).SelectedContent as TreeDataEntry) is not { } treeDataEntry) return;
            TextPreview.Visibility = Visibility.Visible;
            TextPreview.Text = treeDataEntry.EntryDataText;
            
        }
    }
}
