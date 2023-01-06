namespace PboExplorer.Utils.Interfaces;

public interface IPane
{
    string Title { get; }

    bool IsVisible { get; set; }
    
    // TODO: Add other members as needed
}
