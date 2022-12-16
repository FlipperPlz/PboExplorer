using System.IO;
using BisUtils.PBO.Entries;

namespace PboExplorer.Utils;

public sealed class EntryDataStream : MemoryStream {
    public readonly PboDataEntry PboDataEntry;
    public readonly byte[] OriginalDataCRC;

    public EntryDataStream(PboDataEntry pboDataEntry) {
        PboDataEntry = pboDataEntry;
        var pboData = PboDataEntry.EntryData;
        Write(pboData, 0, pboData.Length);
        OriginalDataCRC = CalculateChecksum();
    }

    public bool IsEdited() => CalculateChecksum() != OriginalDataCRC;
    
    public byte[] CalculateChecksum() {
#pragma warning disable SYSLIB0021
        using var sha1 = new System.Security.Cryptography.SHA1CryptoServiceProvider();
#pragma warning restore SYSLIB0021
        return sha1.ComputeHash(ToArray());
    }

    public void SyncToPBO() => PboDataEntry.EntryData = ToArray();
}