using Avalonia.Diagnostics.ViewModels;

namespace Avalonia.Diagnostics.Models;

internal class ConsoleContext
{
	private readonly ConsoleViewModel _owner;

	public readonly string help = "Welcome to Avalonia DevTools. Here you can execute arbitrary C# code using Roslyn scripting.\r\n\r\nThe following variables are available:\r\n\r\ne: The control currently selected in the logical or visual tree view\r\nroot: The root of the visual tree\r\n\r\nThe following commands are available:\r\n\r\nclear(): Clear the output history\r\n";

	public dynamic? e { get; internal set; }

	public dynamic? root { get; internal set; }

	internal static object NoOutput { get; } = new object();

	internal ConsoleContext(ConsoleViewModel owner)
	{
		_owner = owner;
	}

	public object clear()
	{
		_owner.History.Clear();
		return NoOutput;
	}
}
