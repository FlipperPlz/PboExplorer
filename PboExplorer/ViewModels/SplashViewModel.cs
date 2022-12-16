using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

using MRULib;
using MRULib.MRU.Enums;
using MRULib.MRU.Interfaces;
using MRULib.MRU.Models.Persist;
using MRULib.MRU.ViewModels.Base;

using BisUtils.PBO;
using PboExplorer.Windows;


namespace PboExplorer.ViewModels;

internal class SplashViewModel : INotifyPropertyChanged
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

    public event PropertyChangedEventHandler? PropertyChanged;
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

        // TODO: In case of adoption MVVM framework replace MRULib's RelayCommand
        CreatePBOCommand = new RelayCommand<object>(_ => CreateNewPBO());
        OpenPBOCommand = new RelayCommand<object>(_ => OpenPBOFileWithDialog());
        NavigateUriCommand = new RelayCommand<string>(OpenPBOFfile);
        ClearAllItemsCommand = new RelayCommand<object>(_ => ClearMRU());
        RemoveItemCommand = new RelayCommand<object>((p) => RemoveMRUItem(p as IMRUEntryViewModel));
        MovePinnedMruItemUpCommand = new RelayCommand<object>(MovePinnedMruItemUp, CanMovePinnedMruItemUp);
        MovePinnedMruItemDownCommand = new RelayCommand<object>(MovePinnedMruItemDown, CanMovePinnedMruItemDown);
    }

    private void UpdateMRUItem(string path)
    {
        MRUFileList.UpdateEntry(path);
        SaveMRU();
    }

    private void ClearMRU()
    {
        MRUFileList.Clear();
        SaveMRU();
    }

    private void RemoveMRUItem(IMRUEntryViewModel? entry)
    {
        MRUFileList.RemoveEntry(entry);
        SaveMRU();
    }

    // TODO: Make method async in case of adoption AsyncRelayCommand
    private void SaveMRU()
    {
        string persistPath = Path.Combine(_mruFile);
        MRUEntrySerializer.Save(persistPath, MRUFileList);
    }

    private bool CanMovePinnedMruItemUp(object p)
    {
        if (p is not IMRUEntryViewModel ||
           (p as IMRUEntryViewModel)?.IsPinned == 0) //Make sure it is pinned
           return false;
        return true;
    }

    private void MovePinnedMruItemUp(object p)
    {
        if (p is not IMRUEntryViewModel)
            return;

        var param = p as IMRUEntryViewModel;

        MRUFileList.MovePinnedEntry(MoveMRUItem.Up, param);
    }

    private bool CanMovePinnedMruItemDown(object p)
    {
        if (p is not IMRUEntryViewModel ||
           (p as IMRUEntryViewModel)?.IsPinned == 0) //Make sure it is pinned
            return false;

        return true;
    }

    private void MovePinnedMruItemDown(object p)
    {
        if (p is not IMRUEntryViewModel parameters)
            return;

        MRUFileList.MovePinnedEntry(MoveMRUItem.Down, parameters);
    }

    private void CreateNewPBO()
    {
        var dlg = new OpenFileDialog
        {
            Title = "Load PBO archive",
            DefaultExt = ".pbo",
            Filter = "PBO File|*.pbo|Preview BI Files|*.paa;*.rvmat;*.bin;*.pac;*.p3d;*.wrp;*.sqm"
        };
        if (dlg.ShowDialog() != true) return;

        UpdateMRUItem(dlg.FileName);
        NavigateToPboExplorerWindow(new PboFile(dlg.FileName, PboFileOption.Create));
    }

    private void OpenPBOFfile(string path)
    {

        if (string.IsNullOrWhiteSpace(path))
            return;

        try
        {
            UpdateMRUItem(path);
            NavigateToPboExplorerWindow(new PboFile(path));
        }
        catch (Exception exp)
        {
            MessageBox.Show(exp.StackTrace, exp.Message);
        }

    }
    
    private void OpenPBOFileWithDialog()
    {
        var dlg = new OpenFileDialog
        {
            Title = "Load PBO archive",
            DefaultExt = ".pbo",
            Filter = "PBO File|*.pbo|Preview BI Files|*.paa;*.rvmat;*.bin;*.pac;*.p3d;*.wrp;*.sqm"
        };
        if (dlg.ShowDialog() != true) return;

        UpdateMRUItem(dlg.FileName);
        NavigateToPboExplorerWindow(new PboFile(dlg.FileName));
    }

    // TODO: Abstract dependency on Window  
    private void NavigateToPboExplorerWindow(PboFile pbo)
    {
        new PboExplorerWindow(pbo).Show();
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }
}
