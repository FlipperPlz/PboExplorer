using System.IO;
using System.Linq;
using BisUtils.PBO.Entries;

namespace PboExplorer.Utils;

public sealed class EntryDataStream : MemoryStream {
    public readonly PboDataEntry PboDataEntry;
    public readonly byte[] OriginalDataCRC;

    public byte[] EntryData {
        get => ToArray();
        set {
            SetLength(0);
            Write(value, 0, value.Length);
        }
    }

    public EntryDataStream(PboDataEntry pboDataEntry) {
        PboDataEntry = pboDataEntry;
        EntryData = pboDataEntry.EntryData;
        OriginalDataCRC = CalculateChecksum();
    }

    public bool IsEdited() {
        return !CalculateChecksum().SequenceEqual(OriginalDataCRC);
    }
        

    public byte[] CalculateChecksum() {
#pragma warning disable SYSLIB0021
        using var sha1 = new System.Security.Cryptography.SHA1CryptoServiceProvider();
#pragma warning restore SYSLIB0021
        return sha1.ComputeHash(EntryData);
    }

    public async void SyncFromStream(Stream s, bool keepOpen = false) {
        switch (s) {
            case MemoryStream memoryStream: {
                EntryData = memoryStream.ToArray();
                if (!keepOpen) await memoryStream.DisposeAsync();
                break;
            }
            default: {
                using var memoryStream = new MemoryStream();
                await s.CopyToAsync(memoryStream);
                EntryData = memoryStream.ToArray();
                break;
            }
        }
        if(!keepOpen) await s.DisposeAsync();
    }
    
    public void SyncFromPbo() => EntryData = PboDataEntry.EntryData;

    public void SyncToPBO() => PboDataEntry.EntryParent.OverwriteEntryData(PboDataEntry, ToArray(),
        PboDataEntry.EntryMagic == PboEntryMagic.Compressed, true);
}