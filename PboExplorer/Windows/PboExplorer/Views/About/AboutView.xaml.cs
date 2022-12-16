
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace PboExplorer.Windows.PboExplorer.Views.About
{
    /// <summary>
    /// Interaction logic for AboutView.xaml
    /// </summary>
    public partial class AboutView : UserControl
    {
        public AboutView()
        {
            InitializeComponent();
        }

        private void OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            var link = (Hyperlink)sender;
            var navigateUri = link.NavigateUri.ToString();
            Process.Start(new ProcessStartInfo(navigateUri) { UseShellExecute = true }) ;
            e.Handled = true;
        }
    }
}