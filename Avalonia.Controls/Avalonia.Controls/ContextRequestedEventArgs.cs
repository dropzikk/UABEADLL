using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Controls;

public class ContextRequestedEventArgs : RoutedEventArgs
{
	private readonly PointerEventArgs? _pointerEventArgs;

	public ContextRequestedEventArgs()
		: base(Control.ContextRequestedEvent)
	{
	}

	public ContextRequestedEventArgs(PointerEventArgs pointerEventArgs)
		: this()
	{
		_pointerEventArgs = pointerEventArgs;
	}

	public bool TryGetPosition(Control? relativeTo, out Point point)
	{
		if (_pointerEventArgs == null)
		{
			point = default(Point);
			return false;
		}
		point = _pointerEventArgs.GetPosition(relativeTo);
		return true;
	}
}
