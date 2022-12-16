using System;

namespace PboExplorer.Utils.Extensions; 

public static class StringExtensions {
    public static string Truncate(this string value, int maxLength, string truncationSuffix = "…") {
        return value.Length > maxLength
            ? string.Concat(value.AsSpan(0, maxLength), truncationSuffix)
            : value;
    }
}