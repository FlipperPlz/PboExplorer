using System;
using System.Windows.Input;
using MRULib.MRU.ViewModels.Base;
using PboExplorer.Utils.Interfaces;

namespace PboExplorer.Models;

public class AboutEntry : IDocument
{
    public string Title => "About";
    public ICommand CloseCommand { get; }

    public event EventHandler? CloseRequested;

    public AboutEntry()
    {
        CloseCommand = new RelayCommand<object>(_ =>
            CloseRequested?.Invoke(this, EventArgs.Empty)
        );
    }
}