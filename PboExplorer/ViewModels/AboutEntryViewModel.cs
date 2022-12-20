using System;
using System.Windows.Input;
using MRULib.MRU.ViewModels.Base;
using PboExplorer.Models;
using PboExplorer.Utils.Interfaces;

namespace PboExplorer.ViewModels;

public class AboutEntryViewModel : IDocument
{
    public string Title => "About";
    public ICommand CloseCommand { get; }

    public event EventHandler? CloseRequested;

    public AboutEntryViewModel()
    {
        CloseCommand = new RelayCommand<object>(_ =>
            CloseRequested?.Invoke(this, EventArgs.Empty)
        );
    }

    public bool IsDocumentFor(TreeDataEntry entry) => false;
}