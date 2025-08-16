using System.Text;

namespace Avalonia.DesignerSupport.Remote.HtmlTransport;

public class SimpleWebSocketMessage
{
	public bool IsText { get; set; }

	public byte[] Data { get; set; }

	public string AsString()
	{
		return Encoding.UTF8.GetString(Data);
	}
}
