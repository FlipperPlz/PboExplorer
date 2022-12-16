using PboExplorer.Utils.Managers;
using PboExplorer.Utils.Repositories;

namespace PboExplorer.Utils.Interfaces; 

public interface IEntryTreeManaged {
    public EntryTreeManager TreeManager { get; set; }
    public EntryDataRepository DataRepository { get; set; }
    
}