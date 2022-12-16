using System.Collections.Generic;

namespace PboExplorer.Models;

public class FileSearchResult {
    public TreeDataEntry File { get; }
    public IEnumerable<SearchResult> SearchResults { get; }
    
    public FileSearchResult(TreeDataEntry file, IEnumerable<SearchResult> searchResults) {
        File = file;
        SearchResults = searchResults;
    }
}

public class SearchResult {
    public string LineText { get; }
    public int LineNumber { get; }
    public int ColumnNumber { get; }
    public TreeDataEntry File { get; }

    public string AsString => $"{LineNumber}:{ColumnNumber}";
    
    public SearchResult(int lineNumber, int columnNumber, string lineText, TreeDataEntry file) {
        LineNumber = lineNumber;
        ColumnNumber = columnNumber;
        LineText = lineText;
        File = file;
    }
}