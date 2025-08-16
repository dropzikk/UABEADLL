using System.ComponentModel;

namespace Avalonia.Controls;

public class PopulatingEventArgs : CancelEventArgs
{
	public string? Parameter { get; private set; }

	public PopulatingEventArgs(string? parameter)
	{
		Parameter = parameter;
	}
}
