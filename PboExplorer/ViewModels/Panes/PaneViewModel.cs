using CommunityToolkit.Mvvm.ComponentModel;
using PboExplorer.Utils.Interfaces;

namespace PboExplorer.ViewModels;

public abstract partial class PaneViewModel : ObservableObject, IPane
{
    [ObservableProperty]
    protected bool _isVisible = true;

    public abstract string Title { get; }
}
