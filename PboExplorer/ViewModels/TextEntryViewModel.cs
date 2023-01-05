using System.IO;
using System.Text;
using System.Windows;
using PboExplorer.Models;

namespace PboExplorer.ViewModels;

public class TextEntryViewModel : EntryViewModel
{
    private string text;
    private bool isDirty;

    //TODO: Consider moving to base class
    public bool IsDirty
    {
        get => isDirty;
        set
        {
            isDirty = value;
            OnPropertyChanged();
        }
    }
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
    public TextEntryViewModel(TreeDataEntry model, string text):base(model)
    {
        Title = _model.Title;
        Text = text;
    }

    protected override void OnClose()
    {
        if (IsDirty)
        {
            PromptAndSave();
        }
    }

    // TODO: Make async
    protected override void Save()
    {
        var treeManager = _model.TreeManager;
        treeManager.SelectedEntry = _model;
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
        var treeManager = _model.TreeManager;
        treeManager.SelectedEntry = _model;
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
}
