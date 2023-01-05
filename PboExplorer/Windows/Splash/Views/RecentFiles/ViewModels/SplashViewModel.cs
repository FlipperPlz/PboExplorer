using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BisUtils.PBO;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using MRULib;
using MRULib.MRU.Enums;
using MRULib.MRU.Interfaces;
using MRULib.MRU.Models.Persist;
using PboExplorer.Windows.PboExplorer;
using MRURelayCommand = MRULib.MRU.ViewModels.Base.RelayCommand<object>;

namespace PboExplorer.Windows.Splash.Views.RecentFiles.ViewModels;

internal class SplashViewModel
{
    private const string _mruFile = "mru.xml";

    public IMRUListViewModel MRUFileList { get; }

    public ICommand CreatePBOCommand { get; }
    public ICommand OpenPBOCommand { get; }

    public ICommand NavigateUriCommand { get; }
    public ICommand ClearAllItemsCommand { get; }
    public ICommand RemoveItemCommand { get; }
    public ICommand MovePinnedMruItemUpCommand { get; }
    public ICommand MovePinnedMruItemDownCommand { get; }

    public event EventHandler? CloseRequested;

    public SplashViewModel()
    {
        try
        {
            MRUFileList = MRUEntrySerializer.Load(_mruFile);
        }
        catch (FileNotFoundException)
        {
            MRUFileList = MRU_Service.Create_List();
        }

        CreatePBOCommand = new AsyncRelayCommand(CreateNewPBO);
        OpenPBOCommand = new AsyncRelayCommand(OpenPBOFileWithDialog);
        NavigateUriCommand = new AsyncRelayCommand<string>(OpenPBOFile);
        ClearAllItemsCommand = new AsyncRelayCommand(ClearMRU);
        RemoveItemCommand = new AsyncRelayCommand<IMRUEntryViewModel>(RemoveMRUItem);

        // TODO: Solve issue with CanExecute requery
        MovePinnedMruItemUpCommand = new MRURelayCommand(MovePinnedMruItemUp, CanMovePinnedMruItemUp);
        MovePinnedMruItemDownCommand = new MRURelayCommand(MovePinnedMruItemDown, CanMovePinnedMruItemDown);
    }

    private async Task UpdateMRUItem(string path)
    {
        MRUFileList.UpdateEntry(path);
        await SaveMRU();
    }

    private async Task ClearMRU()
    {
        MRUFileList.Clear();
        await SaveMRU();
    }

    private async Task RemoveMRUItem(IMRUEntryViewModel? entry)
    {
        MRUFileList.RemoveEntry(entry);
        await SaveMRU();
    }

    private async Task SaveMRU()
    {
        string persistPath = Path.Combine(_mruFile);
        await MRUEntrySerializer.SaveAsync(persistPath, MRUFileList);
    }

    private bool CanMovePinnedMruItemUp(object p) {
        return p is IMRUEntryViewModel model &&
               model?.IsPinned != 0; //Make sure it is pinned
    }

    private void MovePinnedMruItemUp(object p)
    {
        if (p is not IMRUEntryViewModel model) return;

        MRUFileList.MovePinnedEntry(MoveMRUItem.Up, model);
    }

    private bool CanMovePinnedMruItemDown(object p) {
        return p is IMRUEntryViewModel model &&
               model?.IsPinned != 0; //Make sure it is pinned
    }

    private void MovePinnedMruItemDown(object p)
    {
        if (p is not IMRUEntryViewModel parameters)
            return;

        MRUFileList.MovePinnedEntry(MoveMRUItem.Down, parameters);
    }

    private async Task CreateNewPBO()
    {
        var dlg = new OpenFileDialog
        {
            Title = "Load PBO archive",
            DefaultExt = ".pbo",
            Filter = "PBO File|*.pbo|Preview BI Files|*.paa;*.rvmat;*.bin;*.pac;*.p3d;*.wrp;*.sqm"
        };
        if (dlg.ShowDialog() != true) return;

        await UpdateMRUItem(dlg.FileName);
        NavigateToPboExplorerWindow(new PboFile(dlg.FileName, PboFileOption.Create));
    }

    private async Task OpenPBOFile(string path)
    {

        if (string.IsNullOrWhiteSpace(path))
            return;

        try
        {
            await UpdateMRUItem(path);
            NavigateToPboExplorerWindow(new PboFile(path));
        }
        catch (Exception exp)
        {
            MessageBox.Show(exp.StackTrace, exp.Message);
        }

    }
    
    private async Task OpenPBOFileWithDialog()
    {
        var dlg = new OpenFileDialog
        {
            Title = "Load PBO archive",
            DefaultExt = ".pbo",
            Filter = "PBO File|*.pbo|Preview BI Files|*.paa;*.rvmat;*.bin;*.pac;*.p3d;*.wrp;*.sqm"
        };
        if (dlg.ShowDialog() != true) return;

        await UpdateMRUItem(dlg.FileName);
        NavigateToPboExplorerWindow(new PboFile(dlg.FileName));
    }

    // TODO: Abstract dependency on Window  
    private void NavigateToPboExplorerWindow(PboFile pbo)
    {
        new PboExplorerWindow(pbo).Show();
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }
}
