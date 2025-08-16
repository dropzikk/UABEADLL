using System;

namespace Tmds.DBus.Protocol;

public class ClientSetupResult
{
	public string ConnectionAddress { get; }

	public object? TeardownToken { get; set; }

	public string? UserId { get; set; }

	public bool SupportsFdPassing { get; set; }

	public ClientSetupResult(string address)
	{
		ConnectionAddress = address ?? throw new ArgumentNullException("address");
	}
}
