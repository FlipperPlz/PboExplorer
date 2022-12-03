using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PboExplorer.Windows
{
    /// <summary>
    /// Interaction logic for PboExplorerWindow.xaml
    /// </summary>
    public partial class PboExplorerWindow : Window
    {
        public PboExplorerWindow()
        {
            InitializeComponent();
        }

        private void GridSplitter_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {

        }

        private void SaveAs(object sender, RoutedEventArgs e) {
            throw new NotImplementedException();
        }

        private void Close(object sender, RoutedEventArgs e) {
            throw new NotImplementedException();
        }

        private void CopySelectedEntryName(object sender, RoutedEventArgs e) {
            throw new NotImplementedException();
        }

        private void CopySelectedEntryData(object sender, RoutedEventArgs e) {
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

        private void ShowPboEntry(object sender, RoutedPropertyChangedEventArgs<object> e) {
            throw new NotImplementedException();
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

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) {
            throw new NotImplementedException();
        }
    }
}
