using PboExplorer.Models;

namespace PboExplorer.Messages;

public class ActivateDocumentMessage
{
    public TreeDataEntry Data { get; }

	public ActivateDocumentMessage(TreeDataEntry data)
	{
		Data= data;
	}
}
