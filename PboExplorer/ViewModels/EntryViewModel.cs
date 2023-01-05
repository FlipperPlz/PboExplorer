using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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

    public string Title {
        get => _title;
        set {
            _title = value;
            OnPropertyChanged();
        }
    }
    public ICommand CloseCommand { get; }
    public ICommand SaveCommand { get; }

    public event EventHandler? CloseRequested;


    public EntryViewModel(TreeDataEntry model)
    {
        _model = model;

        // TODO: In case of adoption MVVM framework replace MRULib's RelayCommand
        CloseCommand = new RelayCommand<object>(_ => Close());
        SaveCommand = new RelayCommand<object>(_ => Save());
    }

    public bool IsDocumentFor(TreeDataEntry entry)
       => entry == _model;

    protected virtual void OnClose() { }
    protected abstract void Save();
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
