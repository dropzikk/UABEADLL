using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tmds.DBus.Protocol;

public class ClientConnectionOptions : ConnectionOptions
{
	private string _address;

	public bool AutoConnect { get; set; }

	public ClientConnectionOptions(string address)
	{
		if (address == null)
		{
			throw new ArgumentNullException("address");
		}
		_address = address;
	}

	protected ClientConnectionOptions()
	{
		_address = string.Empty;
	}

	protected internal virtual ValueTask<ClientSetupResult> SetupAsync(CancellationToken cancellationToken)
	{
		return new ValueTask<ClientSetupResult>(new ClientSetupResult(_address)
		{
			SupportsFdPassing = true,
			UserId = DBusEnvironment.UserId
		});
	}

	protected internal virtual void Teardown(object? token)
	{
	}
}
