using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;
using BisUtils.PBO;
using Path = System.IO.Path;

namespace PboExplorer.Windows
{
    /// <summary>
    /// Interaction logic for SplashWindow.xaml
    /// </summary>
    public partial class SplashWindow : Window
    {
        public SplashWindow()
        {
            InitializeComponent();
        }

        private void OpenPBOFile(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Title = "Load PBO archive";
            dlg.DefaultExt = ".pbo";
            dlg.Filter = "PBO File|*.pbo|Preview BI Files|*.paa;*.rvmat;*.bin;*.pac;*.p3d;*.wrp;*.sqm";
            if (dlg.ShowDialog() != true) return;
            
            new PboExplorerWindow(new PboFile(dlg.FileName)).Show();
            Close();

        }

        private void CreateNewPBO(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Title = "Load PBO archive";
            dlg.DefaultExt = ".pbo";
            dlg.Filter = "PBO File|*.pbo|Preview BI Files|*.paa;*.rvmat;*.bin;*.pac;*.p3d;*.wrp;*.sqm";
            if (dlg.ShowDialog() != true) return;
            new PboExplorerWindow(new PboFile(dlg.FileName, PboFileOption.Create)).Show();
            Close();
        }

       
    }
}
