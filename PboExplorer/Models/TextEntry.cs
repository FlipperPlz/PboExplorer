using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MRULib.MRU.ViewModels.Base;
using PboExplorer.Utils.Interfaces;

namespace PboExplorer.Models;

public class TextEntry : INotifyPropertyChanged, IDocument
{
    private readonly TreeDataEntry _dataEntry;
    private string text;

    public string Title { get; set; }
    public string Text
    {
        get => text;
        set
        {
            text = value;
            OnPropertyChanged();
        }
    }
    public ICommand CloseCommand { get; }

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler? CloseRequested;

    public TextEntry(TreeDataEntry dataEntry, string text)
    {
        _dataEntry = dataEntry;
        Title = _dataEntry.Title;

        Text = text;

        // TODO: In case of adoption MVVM framework replace MRULib's RelayCommand
        CloseCommand = new RelayCommand<object>(_ => Close() );
    }

    private void Close()
    {
        // TODO: Check if file edited and prompt save
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    protected void OnPropertyChanged([CallerMemberName] string name = null!)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
