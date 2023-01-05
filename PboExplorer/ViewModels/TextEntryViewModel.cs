using System.IO;
using System.Text;
using System.Windows;
using PboExplorer.Models;
using PboExplorer.Utils;

namespace PboExplorer.ViewModels;

public class TextEntryViewModel : EntryViewModel {
    private string _text;
    
    public string Text
    {
        get => _text;
        set {
            _text = value;
            OnPropertyChanged();
            IsDirty = true;
        }
    }
    
    public TextEntryViewModel(TreeDataEntry model, string text) : base(model, model.Title) => _text = text;

    protected override void OnClose()
    {
        if (IsDirty) PromptAndSave();
    }

    protected override void SaveToPbo() {
        var treeManager = _model.TreeManager;
        treeManager.SelectedEntry = _model;
        var dataStream = treeManager.GetCurrentEntryData().Result;
        SaveToCache(dataStream);
        dataStream.SyncToPBO();
        IsDirty = false;
    }

    protected override void SaveToCache() {
        var treeManager = _model.TreeManager;
        treeManager.SelectedEntry = _model;
        var dataStream = treeManager.GetCurrentEntryData().Result;
        SaveToCache(dataStream);
    }
    
    private void SaveToCache(EntryDataStream entryDataStream) {
        entryDataStream.SyncFromStream(
            new MemoryStream(Encoding.UTF8.GetBytes(Text))
        );
        IsDirty = true;
    }


}
