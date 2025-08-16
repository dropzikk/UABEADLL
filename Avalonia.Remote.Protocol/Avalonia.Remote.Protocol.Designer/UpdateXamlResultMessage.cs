namespace Avalonia.Remote.Protocol.Designer;

[AvaloniaRemoteMessageGuid("B7A70093-0C5D-47FD-9261-22086D43A2E2")]
public class UpdateXamlResultMessage
{
	public string Error { get; set; }

	public string Handle { get; set; }

	public ExceptionDetails Exception { get; set; }
}
