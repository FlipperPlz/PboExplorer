using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

using MRULib.MRU.ViewModels.Base;
using PboExplorer.Models;
using PboExplorer.Utils.Interfaces;

namespace PboExplorer.ViewModels;


/// <summary>
/// Base class for PBO entires view models
/// TODO: use MVVM framework implementation for INotifyPropertyChanged
/// </summary>
public abstract class EntryViewModel : IDocument, INotifyPropertyChanged
{
    protected readonly TreeDataEntry _model;
    private string _title;
    private bool _isDirty;

    public string Title {
        get => _title;
        set {
            _title = value;
            OnPropertyChanged();
        }
    }
    
    protected bool IsDirty {
        get => _isDirty;
        set {
            _isDirty = value;
            OnPropertyChanged();
        }
    }
    
    public ICommand CloseCommand { get; }
    public ICommand SaveCommand { get; }

    public event EventHandler? CloseRequested;


    protected EntryViewModel(TreeDataEntry model, string title)
    {
        _model = model;
        _title = title;

        // TODO: In case of adoption MVVM framework replace MRULib's RelayCommand
        CloseCommand = new RelayCommand<object>(_ => Close());
        SaveCommand = new RelayCommand<object>(_ => SaveToPbo());
    }

    public bool IsDocumentFor(TreeDataEntry entry)
       => entry == _model;

    /// <summary>
    /// Prompt user to save changes before closing
    /// </summary>
    /// <returns> Continue closing or not</returns>
    protected void PromptAndSave() {
        var result = MessageBox.Show("It looks like you've edited this entry, would you like to save it?.\n" +
                                     "Selecting Yes will save the edits of this entry to the corresponding PBO file.\n" +
                                     "Selecting No will save the edits of this entry to cache for later a later sync/edit.\n" +
                                     "Selecting Cancel will revert all changes made.", "PBOExplorer", MessageBoxButton.YesNoCancel);
        switch (result)
        {
            case MessageBoxResult.Yes: SaveToPbo(); break;
            case MessageBoxResult.No: SaveToCache(); break;
            default: DiscardChanges(); break;
        }
    }

    protected virtual void OnClose() {
        if (IsDirty) PromptAndSave();
    }

    protected virtual void DiscardChanges() {
        if(!_isDirty) return;
        var treeManager = _model.TreeManager;
        treeManager.SelectedEntry = _model;
        var dataStream = treeManager.GetCurrentEntryData().Result;
        dataStream.SyncFromPbo();
    }

    protected abstract void SaveToPbo();
    protected abstract void SaveToCache();

    private void Close()
    {
        OnClose();
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    #region INotifyPropertyChanged boilerplate
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null!)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    #endregion
}
