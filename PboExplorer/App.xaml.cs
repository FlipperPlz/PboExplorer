using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Squirrel;

namespace PboExplorer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            SquirrelAwareApp.HandleEvents(
                onInitialInstall: InstallerConfiguration.OnAppInstall,
                onAppUninstall: InstallerConfiguration.OnAppUninstall,
                onEveryRun: InstallerConfiguration.OnAppRun
            );

            await InstallerConfiguration.UpdateApp();
        }
    }
}
