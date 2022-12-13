using Squirrel;
using System.Threading.Tasks;

namespace PboExplorer;

internal static  class InstallerConfiguration
{
    public static void OnAppInstall(SemanticVersion version, IAppTools tools)
    {
        tools.CreateShortcutForThisExe(ShortcutLocation.StartMenu | ShortcutLocation.Desktop);
    }

    public static void OnAppUninstall(SemanticVersion version, IAppTools tools)
    {
        tools.RemoveShortcutForThisExe(ShortcutLocation.StartMenu | ShortcutLocation.Desktop);
    }

    public static void OnAppRun(SemanticVersion version, IAppTools tools, bool firstRun)
    {
        tools.SetProcessAppUserModelId();
    }

    public static async Task UpdateApp()
    {
        using var mgr = new GithubUpdateManager("https://github.com/FlipperPlz/PboExplorer");
        if (mgr.IsInstalledApp)
        {
            await mgr.UpdateApp();
        };
    }
}
