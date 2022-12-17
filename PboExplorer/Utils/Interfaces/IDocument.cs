using PboExplorer.Models;
using System;
using System.Windows.Input;

namespace PboExplorer.Utils.Interfaces;

public interface IDocument
{
    string Title { get; }
    event EventHandler? CloseRequested;
    ICommand CloseCommand { get; }
    
    bool IsDocumentFor(TreeDataEntry entry);
}
