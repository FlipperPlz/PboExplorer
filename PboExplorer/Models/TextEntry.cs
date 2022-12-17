using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using MRULib.MRU.ViewModels.Base;
using PboExplorer.Utils.Interfaces;

namespace PboExplorer.Models;

public class TextEntry : INotifyPropertyChanged, IDocument
{
    private readonly TreeDataEntry _dataEntry;
    private string text;
    private bool isDirty;

    public bool IsDirty
    {
        get => isDirty;
        set
        {
            isDirty = value;
            OnPropertyChanged();
        }
    }
    public string Title { get; set; }
    public string Text
    {
        get => text;
        set
        {
            text = value;
            OnPropertyChanged();
            IsDirty = true;
        }
    }
    public ICommand CloseCommand { get; }
    public ICommand SaveCommand { get; }

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler? CloseRequested;

    public TextEntry(TreeDataEntry dataEntry, string text)
    {
        _dataEntry = dataEntry;
        Title = _dataEntry.Title;

        Text = text;

        // TODO: In case of adoption MVVM framework replace MRULib's RelayCommand
        CloseCommand = new RelayCommand<object>(_ => Close());
        SaveCommand = new RelayCommand<object>(_ => Save());
    }

    private void Close()
    {
        if (IsDirty)
        {
            PromptAndSave();
        }
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    // TODO: Make async
    private void Save()
    {
        var treeManager = _dataEntry.TreeManager;
        treeManager.SelectedEntry = _dataEntry;
        var dataStream = treeManager.GetCurrentEntryData().Result;
        dataStream.SyncFromStream(
            new MemoryStream(Encoding.UTF8.GetBytes(Text))
        );
        if (!dataStream.IsEdited())
        {
            IsDirty = false;
            return;
        }
        dataStream.SyncToPBO();
        IsDirty = false;
    }

    private void DiscardChanges()
    {
        var treeManager = _dataEntry.TreeManager;
        treeManager.SelectedEntry = _dataEntry;
        var dataStream = treeManager.GetCurrentEntryData().Result;
        dataStream.SyncFromPbo();
    }

    /// <summary>
    /// Prompt user to save changes before closing
    /// TODO: Refactor cancellation logic
    /// </summary>
    /// <returns> Continue closing or not</returns>
    private void PromptAndSave()
    {
        var result = MessageBox.Show("It looks like you've edited this entry, would you like to save it?.\n" +
                                   "Selecting Yes will save the edits of this entry to the corresponding PBO file.\n" +
                                   "Selecting No will save the edits of this entry to cache for later a later sync/edit.\n" +
                                   "Selecting Cancel will revert all changes made.", "PBOExplorer", MessageBoxButton.YesNoCancel);
        switch (result)
        {
            case MessageBoxResult.OK:
            case MessageBoxResult.Yes:
                Save();
                break;
            case MessageBoxResult.No:
            case MessageBoxResult.None:
                break;
            case MessageBoxResult.Cancel:
                DiscardChanges();
                break;
            default:
                DiscardChanges();
                break;
        }
    }

    protected void OnPropertyChanged([CallerMemberName] string name = null!)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public bool IsDocumentFor(TreeDataEntry entry) 
        => entry == _dataEntry;
}
