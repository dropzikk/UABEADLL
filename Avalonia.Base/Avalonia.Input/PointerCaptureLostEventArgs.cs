using Avalonia.Interactivity;
using Avalonia.Metadata;

namespace Avalonia.Input;

public class PointerCaptureLostEventArgs : RoutedEventArgs
{
	public IPointer Pointer { get; }

	[Unstable("This constructor might be removed in 12.0. If you need to remove capture, use stable methods on the IPointer instance.,")]
	public PointerCaptureLostEventArgs(object source, IPointer pointer)
		: base(InputElement.PointerCaptureLostEvent)
	{
		Pointer = pointer;
		base.Source = source;
	}
}
